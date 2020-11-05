
local BaseComponent = class("BaseComponent", LuaComponentBase)
_G.BaseComponent = BaseComponent

function BaseComponent:ctor(data)
	self.data = data
	self.cacheAssets = {}
	self.cacheCounts = {}
end

---------Mono方法---------------------------------
function BaseComponent:Awake()
	-- self:BindNodes()
	self:onStart()
	self:addUpdate()
end

function BaseComponent:BindNodes()
	self.nodes = {}
	-- local panel = self.transform:GetComponent(typeof(UIPanel))
	-- if panel then
	-- 	local nodes = panel:getNodes()
	-- 	for i=0,nodes.Length-1 do
	-- 		local node = nodes[i]
	-- 		if node.transform then
	-- 			if node.type == "UnityEngine.GameObject" then
	-- 				self.nodes[node.tag] = node.transform.gameObject
	-- 			else
	-- 				local class = nil
	-- 				local steps = string.split(node.type, ".")
	-- 				for i,v in ipairs(steps) do
	-- 					if class then
	-- 						class = class[v]
	-- 					else
	-- 						class = _G[v]
	-- 					end
	-- 				end
	-- 				if class then
	-- 					local t = typeof(class)
	-- 					if t then
	-- 						self.nodes[node.tag] = node.transform:GetComponent(t)
	-- 					end
	-- 				end
	-- 			end
	-- 		end
	-- 	end
	-- end
end

function BaseComponent:OnDestroy()
	self:removeUpdate()
	self:onDispose()
	self:dispose()
end

-------Mono方法包装------------------------------------------

function BaseComponent:onStart()

end

function BaseComponent:onDispose()
    
end

function BaseComponent:onUpdate()

end

function BaseComponent:onLateUpdate()

end

-- function onFixedUpdate(self)

-- end


-------内部工具方法----------------------------------------------------------------


function BaseComponent:isAlive()
    return not self.disposed
end

function BaseComponent:isShow()
    return self:isAlive() and self.gameObject and self.gameObject.activeSelf
end

--销毁释放
function BaseComponent:dispose()
	self:_disposePlugins()
	self:_disposeCoroutines()
	self:_disposeAssets()
	self:_disposeTweens()
    self:_disposeMembers()
	self.disposed = true
end

function BaseComponent:_disposeMembers()
    self:clearMembers()
end

function BaseComponent:setSprite(image, path, bSetNativeSize)
	local t = os.clock()
	GameAsset.LoadSprite('UI/Sprites/'..path, function(sp)
		if self:isAlive() then
			self.assetSprites = self.assetSprites or {}
			if self.assetSprites[image] then
				GameAsset.ReleaseSingle(image.sprite)
				self.assetSprites[image] = nil
			end
			image.sprite = sp
			if sp then
				self.assetSprites[image] = sp
				if bSetNativeSize then
					image:SetNativeSize()
				end
			end
			-- Log("Load Sprite "..path.." Cost "..(os.clock()-t))
		else
			GameAsset.ReleaseSingle(sp)
		end
    end)
end

function BaseComponent:loadSprite(path, callback)
	local key = "UI/Sprites/"..path
	GameAsset.LoadSprite(key, function(sp)
		if self:isAlive() then
			if callback then
				callback(sp)
				self:addAssetCache(key, sp)
			end
		else
			GameAsset.ReleaseSingle(sp)
		end
    end)
end

function BaseComponent:setTexture(image, path)
	local t = os.clock()
	GameAsset.LoadTexture('UI/Textures/'..path, function(tex)
		if self:isAlive() then
			self.assetTextures = self.assetTextures or {}
			if self.assetTextures[image] then
				GameAsset.ReleaseSingle(image.texture)
				self.assetTextures[image] = nil
			end
			image.texture = tex
			if tex then
				self.assetTextures[image] = tex
			end
			Log("Load Texture "..path.." Cost "..(os.clock()-t))
		else
			GameAsset.ReleaseSingle(tex)
		end
    end)
end

function BaseComponent:loadTexture(path, callback)
	local key = "UI/Textures/"..path
	GameAsset.LoadTexture(key, function(tex)
		if self:isAlive() then
			if callback then
				callback(tex)
				self:addAssetCache(key, tex)
			end
		else
			GameAsset.ReleaseSingle(tex)
		end
    end)
end

function BaseComponent:loadComponent(path, callback)
	local key = "UI/Component/"..path
	GameAsset.LoadPrefab(key, function(prefab)
		if self:isAlive() then
			if callback then
				callback(prefab)
				self:addAssetCache(key, prefab)
			end
		else
			GameAsset.ReleaseSingle(prefab)
		end
    end)
end

--创建组件
function BaseComponent:createComponent(obj, component)
	return UIUtil.createComponent(obj, component)
end

function BaseComponent:addAssetCache(key, asset)
	if not asset then return end

	self.cacheAssets = self.cacheAssets or {}
	self.cacheCounts = self.cacheCounts or {}
	if not self.cacheAssets[key] then
		self.cacheAssets[key] = asset
	end
	self.cacheCounts[key] = self.cacheCounts[key] or 0
	self.cacheCounts[key] = self.cacheCounts[key] + 1
end

function BaseComponent:releaseAsset(key)
	local count = self.cacheCounts[key] or 0
	-- Log(self.__cname.." 释放资源 "..key.." ["..self.cacheCounts[key].."]")
	GameAsset.Release(key, count)
end

function BaseComponent:_disposeAssets(...)
	for k,v in pairs(self.cacheAssets or {}) do
		self:releaseAsset(k)
	end
	for k,v in pairs(self.assetSprites or {}) do
		GameAsset.ReleaseSingle(v)
	end
	for k,v in pairs(self.assetTextures or {}) do
		GameAsset.ReleaseSingle(v)
	end

	self.assetKeys = nil
	self.cacheAssets = nil
	self.cacheCounts = nil
end

--UI组件事件处理挂接包装
function BaseComponent:addEventHandler(event, callback, ...)
	local args = {...}
	event:RemoveAllListeners()
	event:AddListener(function(data)
		if callback then
			callback(self, unpack(args), data) 
		end
    end)
end

--创建协同
function BaseComponent:coroutinePush(func)
	if not self.coroutines_ then
		self.coroutines_ = {}
	end
	table.insert(self.coroutines_,func)
	StartCoroutine(func)
	return #self.coroutines_
end

--销毁协同
function BaseComponent:_disposeCoroutines()
	if self.coroutines_ then
		local count = #self.coroutines_
		for i = count, 1, -1 do
			StopCoroutine(self.coroutines_[i])
		end
		self.coroutines_ = nil
	end
end

--创建缓动
function BaseComponent:tweenPush(tween)
	if not self.tweens_ then
		self.tweens_ = {}
	end
	table.insert(self.tweens_, tween)
	return #self.tweens_
end

function BaseComponent:disposeTween(tween)
	if self.tweens_ then
		table.removeItem(self.tweens_,tween)
		tween:Kill(false)
	end
end

--销毁缓动
function BaseComponent:_disposeTweens()
	if self.tweens_ then
		local count = #self.tweens_
		
		for i=count, 1, -1 do
			if self.tweens_[i] then
				self.tweens_[i]:Kill(false)
			end
		end
		self.tweens_ = nil
	end
end

-- 添加插件
function BaseComponent:addPlugin(plugin)
	if not self.plugins_ then
		self.plugins_ = {}
	end
	table.insert(self.plugins_, plugin)
	return plugin
end

-- 卸载插件
function BaseComponent:_disposePlugins()
	if self.plugins_ then
		for _,v in ipairs(self.plugins_) do
			v:dispose()
		end
		self.plugins_ = nil
	end
end

-------------------------------------------------

--读取UI组件尺寸 Vector2
function BaseComponent:getSize()
	local transform = self.gameObject:GetComponent(typeof(RectTransform))
	if transform then
		return transform.sizeDelta
	else
		return nil
	end
end

--读取UI组件宽度
function BaseComponent:getWidth()
	local transform = self.gameObject:GetComponent(typeof(RectTransform))
	if transform then
		return transform.sizeDelta.x
	else
		return 0
	end
end

--读取UI组件高度
function BaseComponent:getHeight()
	local transform = self.gameObject:GetComponent(typeof(RectTransform))
	if transform then
		return transform.sizeDelta.y
	else
		return 0
	end
end
