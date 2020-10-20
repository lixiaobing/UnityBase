-- @brief: UI通用工具
-- @version: 1.0.0
-- @author lanqibo
-- All rights reserved
UIUtil = {}
local this = UIUtil

function UIUtil.createComponent(obj, component)
	local cpn = obj.transform:GetComponent(typeof(LuaComponent))
	local tb = nil

	if cpn then
		tb = cpn:GetLuaTable()
		if tb.__cname ~= component.__cname then
			tb = component.new()
			cpn:SetLuaTable(tb)
		end
		return tb
	end

	tb = component.new()
	tb:AddLuaComponent(obj)

	return tb
end

function UIUtil.cloneGameObject(obj)
    if not obj then
        return
    end
    local goParent = null
    local transParent = obj.transform.parent
    if transParent then
        goParent = transParent.gameObject
    end
    local newObj = GameObject.Instantiate(obj)
    if newObj then
        newObj.name = obj.name
        if goParent then
            local trans = newObj.transform
            trans:SetParent(transParent)
            trans.localPosition = Vector3.zero
            trans.localRotation = Quaternion.identity
            trans.localScale = Vector3.one
            newObj.layer = goParent.layer
        end
        newObj:SetActive(true)
    end
    return newObj
end

--根据字符串颜色码获取Color对象
function UIUtil.getColor(color)
	local b,c = ColorUtility.TryParseHtmlString(color, nil)
    if b then
		return c
	else
		return Color.white
	end
end

--图片置灰
function UIUtil.setGrey(img, isGrey)
    if isGrey then
        local _shader = Shader.Find("UI/ImageGrey");
        local _mat = Material(_shader)
        img.material = _mat
    else
        img.material = nil
    end
end

--带遮罩支持的图片置灰
function UIUtil.setGrayWithStencil(img, isGrey)
    if isGrey then
        local _shader = Shader.Find("UI/ImageGrayWithStencil");
        local _mat = Material(_shader)
        img.material = _mat
    else
        img.material = nil
    end
end

--- 设置资源图
-- @param image  要设置到的目标
-- @param path   路径
-- @param spriteName  图片名称
-- @param bSetNativeSize  是否 SetNativeSize
function UIUtil.setSprite(image, path, bSetNativeSize)
    GameAsset.LoadSprite('UI/Sprites/'..path, function(sp)
        image.sprite = sp
        if bSetNativeSize then
            image:SetNativeSize()
        end
    end)
end

function UIUtil.loadSprite(path, callback)
    GameAsset.LoadSprite('UI/Sprites/'..path, function(sp)
        if callback then
            callback(sp)
        end
    end)
end

function UIUtil.loadEffect(path, parent)
    GameAsset.LoadPrefab("Effect/UI/"..path, function(prefab)
        if prefab then
            this.addEffect(prefab, parent, true)
        end
    end)
end

function UIUtil.addEffect(prefab, parent, destroy, position, rotation)
    local obj = GameObject.Instantiate(prefab)

    Prefab.setLayer(obj, UILayerNames.UI)

    if parent then
        obj.transform:SetParent(parent, false)
        obj:SetActive(true)

        obj.transform.localPosition = position or Vector3.zero
        obj.transform.localEulerAngles = rotation or Vector3.zero
    end

    local eff = obj.transform:GetComponent(typeof(UIEffectSort))
    if not eff then
        obj:AddComponent(typeof(UIEffectSort))
    end

    local ctrl = obj.transform:GetComponent(typeof(EffectControll))
    if ctrl then
        obj:SetActive(true)
        ctrl:Play()
        if destroy then
            ctrl.onFinished:AddListener(function()
                GameObject.Destroy(obj)
            end)
        end
    else
        LogError("特效".."没有绑定EffectControll")
    end

    return ctrl
end

--主要用于UI循环特效
function UIUtil.createLoopEffect(uibody, tagName, prefab, parent, isShow, reStart)
    uibody.loopEffects = uibody.loopEffects or {}
    local pool = uibody.loopEffects
    local tag = tagName or prefab.name
    if isShow then
        if not pool[tag] then
            pool[tag] = UIUtil.addEffect(prefab, parent, false)
        else
            if reStart then
                pool[tag].gameObject:SetActive(false)
            end
            pool[tag].gameObject:SetActive(true)
        end
    else
        if pool[tag] then
            pool[tag].gameObject:SetActive(false)
        end
    end
end

--设置角色品质
function UIUtil.setSpriteStar(image,star)
    UIUtil.setSprite(image,SpriteStarIcon[star])
end

--设置物品icon
function UIUtil.setItemIcon(image,pathIcon)
    local path = string.format("%s%s", BagManager.pathItemIcon, pathIcon)
    -- Log("setItemIcon path:"..path)
    UIUtil.setSprite(image, path)
end

--设置物品背景
function UIUtil.setItemSmallBackground(image,background)
    local path = string.format("%sQuality_Samll%02d", BagManager.pathItemQuality, background) 
    --Log("setItemBackground path:"..path)
    UIUtil.setSprite(image,path)
end

function UIUtil.setItemBigBackground(image,background)
    local path = string.format("%sQuality_Big%02d", BagManager.pathItemQuality, background) 
    --Log("setItemBackground path:"..path)
    UIUtil.setSprite(image,path)
end
--设置关卡图标
function UIUtil.setLevelIcon(image,path)
    path = string.format("Icon/Level/%s", path) 
    UIUtil.setSprite(image,path)
end

function UIUtil.setTexture(image, imgName)
    GameAsset.LoadTexture("UI/Textures/"..imgName, function(tex)
        image.texture = tex
        image.color   = Color(1,1,1,1)
    end)
end

--传入统一ID，返回当前数量，包含特殊道具
function UIUtil.getItemCount(id)
    if id == ItemType.GOLD then
        return BagMgr:getGoldCount()
    elseif id == ItemType.DIAMOND then
        return BagMgr:getDemondCount()
    else
        return BagMgr:getItemDataByCId(id).num
    end
end

--显示关卡奖励弹出界面（不要调用OpenWindow）
function UIUtil.showBoxRewards(rewards)
    local win = UIManager.getWindow("BoxRewardWindow")
    if win then
        win:append(rewards)
    else
        UIManager.openWindow('BoxRewardWindow', nil, rewards)
    end
end