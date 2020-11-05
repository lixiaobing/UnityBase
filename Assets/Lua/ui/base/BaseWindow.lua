---@class BaseWindow : BaseComponent
local BaseWindow = class("BaseWindow", BaseComponent)
_G.BaseWindow = BaseWindow

--首次创建初始化
function BaseWindow:Awake()
    if self.canvasGroup then
        self.canvasGroup.alpha = 0
    end
	self:BindNodes()
	self:onStart()
end

function BaseWindow:onStart()
    StartCoroutine(function()
        self:onAsset()
        self:onLoadAsset()
        self:onPluginBase()
        self:onInit()
        --首次打开延迟三帧（预留图标加载时间）
        Yield(0)
        Yield(0)
        Yield(0)
        self:show()
        self:addUpdate()
        self.isInited = true
	end)
end

--准备基础插件
function BaseWindow:onPluginBase()
	--self.messager = self:addPlugin(Messager.new(self))
end

---------UI流程方法------------------------------------------------------

function BaseWindow:onAsset()
    --废弃之前的onResource（方便统一管理UI加载的资源）
    --self.assetKeys 指定需要加载的资源key（key和Addressables里面对应）
    --self.assetComplete 单个资源加载完的回调
    --若没有提供self.assetComplete，则在主逻辑中可用self.cacheAssets[key]获取对应资源
end

function BaseWindow:onLoadAsset() 
    self.assetKeys = self.assetKeys or {}
    if next(self.assetKeys) then
        Yield(GameAsset.LoadAssets(self.assetKeys, function(index, obj)
            local key = self.assetKeys[index]
            self:addAssetCache(key, obj)

            if self.assetComplete then
                self.assetComplete(index, obj)
            end
        end))
    end
end

--2.初始化UI（第一次打开UI处理，用于界面初始化:组件关联、变量赋值，事件绑定）
function BaseWindow:onInit() 

end

--3.打开UI（打开/刷新UI处理）
function BaseWindow:onOpen()
	
end

--5.关闭UI（UIManager销毁UI前处理）
function BaseWindow:onClose()

end

function BaseWindow:onHide()

end

function BaseWindow:onDispose()
end

function BaseWindow:onOrderChanged(newSort, allCount)
    -- Log(self.Config.name.." "..newSort.." "..allCount)
end

--隐藏UI
function BaseWindow:hide()
    if self.Config then
        local cavas = self.transform:GetComponent(typeof(CanvasGroup))
        cavas.alpha = 0 
        self.gameObject:SetActive(false)
        self:removeUpdate()
        self:onHide()
	end
end

--显示UI
function BaseWindow:show()
    if self.Config and self:isAlive() then
        local doShow = function()
            
            self.gameObject:SetActive(true)
            self.canvasGroup.alpha = 1

            UIManager.closeSameLayerWindow(self)
            UIManager.setCoverLayer()
            UIManager.controllMainState()

            if self.onComplete then
                self.onComplete()
                self.onComplete = nil
            end

            -- LogError(self.__cname.." SHOW!!!")

            self:addUpdate()
            EventCenter.sendMessage(EventType.OPEN_WINDOW_FINISH, self.Config)
            self:onOpen()
        end

        if self.Config.layer == UIConfig.LayerType.Screen then
            local bgName = UIConfig.BgConfig[self.Config.name]
            if bgName then
                UIManager.loadBackground(bgName, function()
                    doShow()
                end)
            else
                UIManager.clearBackground()
                doShow()
            end
        else
            doShow()
        end
	end
end

function BaseWindow:close(onComplete)
    if self.Config then
        local closeHandler = self.closeHandler
        if closeHandler then
            self.closeHandler = nil
            closeHandler(function()
                self:close(onComplete)
            end)
            return
        end
        UIManager.closeWindow(self.Config.name, function()
            if onComplete then
                onComplete()
            end
            EventCenter.sendMessage(EventType.CLOSE_WINDOW_FINISH, self.Config)
        end)
	end
end

function BaseWindow:setCloseHandler(handler)
    self.closeHandler = handler
end

function BaseWindow:setCoverCallBack(callback)
    self.coverCallBack = callback
    local winBtn = self.transform:GetComponent(typeof(Button))
    if not winBtn then
        winBtn = self.gameObject:AddComponent(typeof(Button))
        winBtn.transition = Transition.None
    end
end