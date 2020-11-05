-- @brief: UI管理器
-- @version: 1.0.0
-- @author lanqibo
-- All rights reserved
-- module("UIManager", package.seeall)
UI = {}
UI.Canvas = nil
UI.Camera = nil
UI.Rect = nil
UI.Model = nil
UI.BackgroundImage = nil
UI.BackgroundRect = nil
UI.BgmPlayer = nil
UI.isLock = false

UIManager = {}
local this = UIManager

--窗口序列栈
local _OrderStacks = {}
--窗口加载栈
local _AwaitStacks = {}
--窗口缓存栈
local _CacheStacks = {}
--窗口Prefab栈
local _PrefabCache = {}
local _PrefabCacheAlways = {}

local MAX_CACHE_STACK_SIZE = 5
--Prefab最高缓存栈
local MAX_PREFAB_CACHE_SIZE = 5

local _Layers = {}

local _Messager = nil

local _PopupGroup = {
    UIConfig.LayerType.Popup1,
    UIConfig.LayerType.Popup2,
    UIConfig.LayerType.Popup3,
    UIConfig.LayerType.Popup4,
    UIConfig.LayerType.Popup5
}

local _PopupMap = {}

local _BackgroundCache = {}

-- --刘海适配开关 todo
local NOTCH_TOGGLE = true
local NOTCH_SIZE = 60

--UI适配模式  Expand全拉伸 ExpandX横向拉伸 ExpandY纵向拉伸
local FIX_MODE = "Expand"

function UIManager.getResolution()
    return Screen.width/Screen.height
end

function UIManager.init()
	local ui = GameObject.Find("UI")
    local uiCanvas = GameObject.Find("UI/Canvas")

    UI.Canvas = GameObject.Find("UI/Canvas"):GetComponent(typeof(Canvas))
    UI.Camera = GameObject.Find("UI/Camera"):GetComponent(typeof(Camera))
    UI.Rect = GameObject.Find("UI/Canvas"):GetComponent(typeof(RectTransform))
    UI.Model = GameObject.Find("UIModel")
    UI.BgmPlayer = GameObject.Find("UI/Camera"):GetComponent(typeof(AudioSource))
    UI.BackgroundImage = GameObject.Find("UI/BgCanvas/Background"):GetComponent(typeof(RawImage))
    UI.BackgroundRect = UI.BackgroundImage.transform:GetComponent(typeof(RectTransform))

	-- 创建UI分层容器
	local layer
    for i = 1, #UIConfig.Layers do
		layer = GameObject(UIConfig.Layers[i])
        layer:AddComponent(typeof(RectTransform))
        layer.transform:SetParent(uiCanvas.transform, false)
        layer.transform.localPosition = Vector3.zero
        layer.transform.localScale = Vector3.one

        layer.transform.anchorMin = Vector2.zero
        layer.transform.anchorMax = Vector2.one
        layer.transform.offsetMax = Vector2.zero
        layer.transform.offsetMin = Vector2.zero

        layer.layer = uiCanvas.layer

        local layer_canvas = layer:AddComponent(typeof(Canvas))

        local include = false
        for _,l in ipairs(UIConfig.CoverLayers) do
            if l==i then
                include = true
                break
            end
        end

        if include then
            local layer_image = layer:AddComponent(typeof(Image))
            layer_image.enabled = false
            layer_image.color = Color(0,0,0,0.5)
            local layer_button = layer:AddComponent(typeof(Button))
            layer_button.transition = Transition.None
            layer_button.enabled = false
        end

        local layer_raycast = layer:AddComponent(typeof(GraphicRaycaster))
        layer_canvas.overrideSorting = true
        layer_canvas.sortingLayerName = UIConfig.Layers[i]
        layer_canvas.sortingOrder = -1

        _Layers[i] = layer.transform
	end

    local lockLayer = _Layers[UIConfig.LayerType.Lock]
    local layer_image = lockLayer.gameObject:AddComponent(typeof(Image))
    layer_image.color = Color(0,0,0,0)
    local layer_button = lockLayer.gameObject:AddComponent(typeof(Button))
    layer_button.transition = Transition.None
    lockLayer.gameObject:SetActive(false)

    local darkLayer = _Layers[UIConfig.LayerType.Dark]
    local layer_image = darkLayer.gameObject:AddComponent(typeof(Image))
    layer_image.color = Color(0,0,0,0)
    local layer_button = darkLayer.gameObject:AddComponent(typeof(Button))
    layer_button.transition = Transition.None
    darkLayer.gameObject:SetActive(false)

    --UIGlobal.init()

    if this.onUpdate then
		UpdateBeat:Add(this.onUpdate, this)
	end
end

function UIManager.onUpdate()
    this.lastWidth = this.lastWidth or 0
    this.lastHeight = this.lastHeight or 0

    local isChange = false
    if Screen.width ~= this.lastWidth or Screen.height ~= this.lastHeight then
        isChange = true
        this.lastWidth = Screen.width
        this.lastHeight = Screen.height

        Debug.Log(string.format("Screen Resolution:%s*%s", Screen.width, Screen.height))
    end

    if NOTCH_TOGGLE and isChange then
        local resolution = this.getResolution()
        local layers = {UIConfig.LayerType.Screen, UIConfig.LayerType.ScreenUp}

        for i,layerName in ipairs(layers) do
            local layer = _Layers[layerName]

            local rect = layer:GetComponent(typeof(RectTransform))
            if resolution>=2 then
                if Screen.width > Screen.height then
                    layer.transform.offsetMax = Vector2(-NOTCH_SIZE, 0)
                    layer.transform.offsetMin = Vector2(NOTCH_SIZE, 0)
                else
                    layer.transform.offsetMax = Vector2(0, -NOTCH_SIZE)
                    layer.transform.offsetMin = Vector2(0, NOTCH_SIZE)
                end
            else
                layer.transform.offsetMax = Vector2.zero
                layer.transform.offsetMin = Vector2.zero
            end
        end
    end    
end

function UIManager.lockScreen(isLock)
    local lockLayer = _Layers[UIConfig.LayerType.Lock]
    lockLayer.gameObject:SetActive(isLock)

    UI.isLock = isLock
end

function UIManager.loadBackground(imgName, callback)
    --Log.debug("loadBackground=========="..imgName)
    if _BackgroundCache[imgName] then
        UI.BackgroundImage.texture = _BackgroundCache[imgName]
        UI.BackgroundImage.color = Color(1,1,1,1)
        if callback then
            callback()
        end
    else
        GameAsset.LoadTexture("UI/Textures/"..imgName, function(tex)
            _BackgroundCache[imgName] = tex

            UI.BackgroundImage.texture = tex
            UI.BackgroundImage.color = Color(1,1,1,1)

            if callback then
                callback()
            end
        end)
    end
end

function UIManager.clearBackground()
    UI.BackgroundImage.texture = nil
    UI.BackgroundImage.color = Color(1,1,1,0)
end

--------------------------------------------------↓窗口逻辑↓--------------------------------------------------------
function UIManager.openWindow(winName, onComplete, ...)
    local data = {...}
    local set = UIConfig.Settings[winName]

    local time = os.clock()

    local callback = function()
        if onComplete then
            onComplete()
        end
        -- StartCoroutine(function()
        --     Yield(0)
    
        --     GuideManager.checkWindow(winName)
        -- end)
        
        Log("===============OPEN WINDOW COMPLETE: "..winName.." "..set.layer.." "..(os.clock()-time))
    end

    this.doOpenWindow(winName, callback, data)
end

function UIManager.openWindowQueue( winNames, onAllComplete)
    if not next(winNames) then
        if onAllComplete then
            onAllComplete()
        end
        return
    end
    this.openWindow(winNames[1], function()
        table.remove(winNames, 1)
        this.openWindowQueue(winNames, onAllComplete)
    end)
end

--打开指定Window
function UIManager.doOpenWindow(winName, onComplete, data)
    -- Log(#UIConfig.Settings)
	local set = UIConfig.Settings[winName]
	if not set then
        LogError("opening a non-existent window = "..winName)
        return nil
    end
    local count = #_OrderStacks

    -- 检查当前是否存在于队列，存在则置顶层
    for i = count, 1, -1 do
        local win = _OrderStacks[i]
        if win and win.Config.name == winName then
            win.transform:SetAsLastSibling()
            win.gameObject:SetActive(false)
            win.onComplete = onComplete
            table.remove(_OrderStacks, i)
            table.insert(_OrderStacks, win)

            this.recordPopupMap(win.Config.layer, true)
            win:show()
            this.doOrderStacks()
            -- if onComplete then
            --     onComplete()
            -- end
            return
		end
    end

    -- 检查当前是否存在缓存池，存在则调出
    for i,win in ipairs(_CacheStacks) do
        if win.Config.name == winName then
            local layerTrans, layer = this.getLayer(win.Config.layer)

            win.transform:SetParent(layerTrans, false)
            win.transform:SetAsLastSibling()
            win.gameObject:SetActive(false)
            win.onComplete = onComplete
            win.trueLayer = layer
            win.canvas.sortingLayerName = layerTrans.gameObject.name

            table.remove(_CacheStacks, i)
            table.insert(_OrderStacks, win)
            this.recordPopupMap(layer, true)
            
            win.data = data
            win:onInit()
            win:show()

            this.doOrderStacks()
            return
        end
    end

	--都不存在则加载UI
	for _,v in ipairs(_AwaitStacks) do
        local break_open = false
        if v.name == winName then
            break_open = true
        -- elseif v.layer == set.layer then
        --     break_open = true
        end
        if break_open then 
            return 
        end
    end
    table.insert(_AwaitStacks, set)

    local prefab = this.getPrefabCache(winName)
    if prefab then
        this.doCreateWindow(prefab, set, data, onComplete)
        this.doRemoveAwait(winName)
    else
        GameAsset.LoadPrefab(set.path, function(obj)
            if obj then
                this.doCreateWindow(obj, set, data, onComplete)
                this.addPrefabCache(winName, obj)
            else
                if not obj then
                    LogError(string.format("Prefab.LoadPrefab is loading failed for window’s name : %s", winName))
                end
            end
            this.doRemoveAwait(winName)
        end)
    end
end

function UIManager.waitOpenWindow(winName, data)
    local complete = false
    this.openWindow(winName, function()
        complete = true
    end, data)
    while not complete do
        Yield(0)
    end
end

function UIManager.doRemoveAwait(winName)
    for i,v in ipairs(_AwaitStacks or {}) do
        if v.name == winName then
            table.remove(_AwaitStacks, i)
            break
        end
    end
end

--封装窗口加载回调函数
function UIManager.doCreateWindow(obj, set, data, onComplete)
    --实例化Window Prefab
    local winobj = GameObject.Instantiate(obj)
    local canvas = winobj:GetComponent(typeof(Canvas))
    if not canvas then
        canvas = winobj:AddComponent(typeof(Canvas))
    end
    if not winobj:GetComponent(typeof(GraphicRaycaster)) then
        winobj:AddComponent(typeof(GraphicRaycaster))
    end        
    local canvasGroup = winobj:GetComponent(typeof(CanvasGroup))
    if not canvasGroup then
        canvasGroup = winobj:AddComponent(typeof(CanvasGroup))
    end

    Log("=======Create Window："..set.name)

    local layerTrans, layer = this.getLayer(set.layer)

    --实例化Window控制器
    local wincpn = set.class.new(data)
    winobj.transform:SetParent(layerTrans.transform, false)
    winobj.name = set.name    
    winobj:SetActive(false)
    
    wincpn.Config = set
    wincpn.onComplete = onComplete
    wincpn.canvas = canvas
    wincpn.canvas.overrideSorting = true
    wincpn.canvas.sortingLayerName = layerTrans.gameObject.name
    wincpn.canvasGroup = canvasGroup
    wincpn.trueLayer = layer

    table.insert(_OrderStacks, wincpn)
    this.recordPopupMap(layer, true)

    this.doOrderStacks()
    wincpn:AddLuaComponent(winobj)
end

function UIManager.recordPopupMap(layer, isMark)
    local include = false
    for i,v in ipairs(_PopupGroup) do
        if v == layer then
            include = true
            break
        end
    end
    if include then
        _PopupMap[layer] = isMark
    end
end

function UIManager.getLayer(layer)
    if layer == UIConfig.LayerType.Popup then
        for i,v in ipairs(_PopupGroup) do
            local layerTrans = _Layers[v]
            if not _PopupMap[v] then
                return layerTrans, v
            end
        end
    end
    return _Layers[layer], layer
end

function UIManager.getPrefabCache(winName)
    local prefab = nil
    for _, data in ipairs(_PrefabCacheAlways) do
        if data[1] == winName then
            prefab = data[2]
        end
    end
    if not prefab then
        for _, data in ipairs(_PrefabCache) do
            if data[1] == winName then
                prefab = data[2]
            end
        end
    end
    return prefab
end

function UIManager.addPrefabCache(winName, prefab, always)
    local cache = this.getPrefabCache(winName)
    if not cache then
        if always then
            table.insert(_PrefabCacheAlways, {winName, prefab})
        else
            local size = #_PrefabCache
            if size >= MAX_PREFAB_CACHE_SIZE then
                local cache = _PrefabCache[1]
                cache[2] = nil
                table.remove(_PrefabCache, 1)
            end
            table.insert(_PrefabCache, {winName, prefab})
        end
    end
end

function UIManager.closeSameLayerWindow(win)
    local set = win.Config
    local layer = win.transform.parent.gameObject.name
    local count = #_OrderStacks
    for i = count, 1, -1 do
        local winLayer = _OrderStacks[i].transform.parent.gameObject.name
        if winLayer == layer then
            if _OrderStacks[i].Config.name ~= set.name then
                _OrderStacks[i]:hide()
            end
        end
    end
end

--关闭指定Window
function UIManager.closeWindow(winName, onComplete)
    Log("===============CLOSE WINDOW: "..winName)
    local count = #_OrderStacks
    for i=count,1,-1 do
        local window = _OrderStacks[i]
        if window then
            if window.Config.name == winName then
                window:removeUpdate()
                window:onClose()
                table.remove(_OrderStacks, i)
                this.doCacheOrDestroy(window)
                this.doOrderStacks()

                this.doReActiveTop()
                this.doReActiveSameLayer(window.Config.layer)
                -- controllMainState()

                this.setCoverLayer()
            
                if onComplete then
                    onComplete()
                end
                break
            end
        end
    end
end

--移动window到缓存池
function UIManager.doCacheOrDestroy(window)
    if window.Config.cache then
        -- Log.debug(window.Config.name)
        -- Log.debug(window.Config.cache)
        local poolLayer = _Layers[UIConfig.LayerType.Pool]

        window:hide()
        window.transform.localScale = Vector3.one
        window.transform:SetParent(poolLayer, false)

        this.recordPopupMap(window.trueLayer, false)

        table.insert(_CacheStacks, window)
        
        if #_CacheStacks > MAX_CACHE_STACK_SIZE then
            local window = _CacheStacks[1]
            GameObject.Destroy(window.gameObject)
            table.remove(_CacheStacks, 1)
        end
    else
        GameObject.Destroy(window.gameObject)
        this.recordPopupMap(window.trueLayer, false)
        -- window:_dispose()
    end
end

--后退
function UIManager.back(onComplete)
	local count = #_OrderStacks
    local idx=1
	for i = count, 1, -1 do
		local win = _OrderStacks[i]
		if win and win.gameObject and win.gameObject.activeSelf then
            local config = win.Config
            idx = i
            break
        end
    end

    local win = _OrderStacks[idx]
    this.doBack(win, idx, onComplete)
end

function UIManager.doBack(window, idx, onComplete)
    window:onClose()
    table.remove(_OrderStacks, idx)
    this.doOrderStacks()

    this.doCacheOrDestroy(window)
    this.doReActiveTop()
    this.doReActiveSameLayer(window.Config.layer)

    if onComplete then
        onComplete()
    end
end

function UIManager.doReActiveTop()
    local top = #_OrderStacks
    local topWindow = _OrderStacks[top]
    if topWindow then
        if topWindow.gameObject.activeSelf then
            if not topWindow.onComplete then
                topWindow:onOpen()
            end
        end
    end
end

function UIManager.doReActiveSameLayer(layer)
    local count = #_OrderStacks

    for i = count, 1, -1 do
        local window = _OrderStacks[i]
        if window.Config.layer ~= UIConfig.LayerType.Popup and window.Config.layer == layer and not window.gameObject.activeSelf then
            window:show()
            --GuideManager.checkWindow(window.Config.name)
            break
        end
    end
end

function UIManager.closeAllWindow(...)
    Log("===============CLOSE ALL WINDOW: ")

    local excepts = {...}
    local count = #_OrderStacks

    local cacheNames = {}
    for i,v in ipairs(_CacheStacks) do
        cacheNames[v.Config.name] = true
    end

	for i = count, 1, -1 do
        local window = _OrderStacks[i]
        -- Log.debug(window.Config.name)
        -- Log.debug(not arrayContain(excepts, window.Config.name))
        -- Log.debug(not cacheNames[window.Config.name])
        if not cacheNames[window.Config.name] and not arrayContain(excepts, window.Config.name) then
            window:onClose()
            table.remove(_OrderStacks, i)
            this.doCacheOrDestroy(window)

            this.doOrderStacks()
        end
    end
    this.setCoverLayer()
    -- controllMainState()
    EventCenter.sendMessage(EventType.SHOW_TOPBANNER,false) --隐藏topbanner
end

function UIManager.hideAllWindow(...)
    local excepts = {...}
    local count = #_OrderStacks

    local recordStates = {}

	for i = count, 1, -1 do
        local window = _OrderStacks[i]

        if window.gameObject.activeSelf and not arrayContain(excepts, window.Config.name) then
            recordStates[window.Config.name] = true
            window:hide()
        end
    end
    this.setCoverLayer()
    -- controllMainState()

    return recordStates
end

function UIManager.recoverWindow(states)
    local count = #_OrderStacks
    for i = count, 1, -1 do
        local window = _OrderStacks[i]
        if states[window.Config.name] then
            window:show()
        end
    end
end

function UIManager.setCoverLayer()
    for i=#UIConfig.CoverLayers,1,-1 do
        local sort = UIConfig.CoverLayers[i]
        local layer = _Layers[sort]

        local img = layer:GetComponent(typeof(Image))
        local btn = layer:GetComponent(typeof(Button))
        img.enabled = false
        
        btn.onClick:RemoveAllListeners()
        btn.enabled = false
    end

    local count = #_OrderStacks
	for i = count, 1, -1 do
		local window = _OrderStacks[i]
        local winLayer = UIConfig.getLayerTypeByName(window.transform.parent.gameObject.name)

        local include = false
        for _,layer in ipairs(UIConfig.CoverLayers) do
            if window.gameObject.activeSelf and winLayer==layer then
                include = true
                break
            end
        end
        if include then
            local layer = _Layers[winLayer]
            local img = layer:GetComponent(typeof(Image))
            img.color = UIConfig.CoverColors[window.Config.name] or UIConfig.CoverDefaultColor

            local btn = layer:GetComponent(typeof(Button))

            img.enabled = true
            btn.enabled = true

            btn.onClick:RemoveAllListeners()
            btn.onClick:AddListener(function() 
                if window.isInited and window.coverCallBack then
                    window:coverCallBack()
                end
            end)
            break
        end
    end
end

function UIManager.setWindowCloseHandler(winName, handler)
    local win = this.getWindow(winName)
    if win then
        win.closeHandler = handler
    end
end

function UIManager.controllMainState()
    -- local haveScreen = false
    -- local count = #_OrderStacks
    -- for i = count, 1, -1 do
    --     local window = _OrderStacks[i]
	-- 	if window and window.gameObject.activeSelf and window.Config.layer == UIConfig.LayerType.Screen then
    --         haveScreen = true
    --         break
	-- 	end
    -- end

    -- showMainBar(not haveScreen)
end

--获得指定当前正在运行的Window组件
function UIManager.getWindow(winName)
	local count = #_OrderStacks
    for i = count, 1, -1 do
        local window = _OrderStacks[i]
		if window and window.Config.name == winName and window.gameObject then
			return window
		end
	end
	return nil
end

function UIManager.doOrderStacks()
    local str = ""
    local count = #_OrderStacks
    for i = count, 1, -1 do
        local window = _OrderStacks[i]
        window:onOrderChanged(i, count)

        str = str .. window.Config.name.."->"
    end

    Log("OrderStacks:"..str)
end

function UIManager.getTopScreen()
    local count = #_OrderStacks
    for i = count, 1, -1 do
        local window = _OrderStacks[i]
        if window.Config.layer == UIConfig.LayerType.Screen then
            return window
        end
    end
    return nil
end

local DefaultMargin =  {0,0,0,0}
--世界坐标->屏幕坐标->UI坐标
function  UIManager.worldToLocalPoint(camera ,worldPos ,margin)
    if not margin then 
        margin = DefaultMargin
    end
    local screenPoint = RectTransformUtility.WorldToScreenPoint(camera, worldPos)
    local forward     = camera.transform.forward
    local targetDir   = worldPos - camera.transform.position
    local angle       = Vector3.Angle(targetDir, forward)
    if angle > 90 then --解决在相机背后位置不
        screenPoint  = screenPoint * -1
    end
    local rectTransform    = UI.Rect
    local ret , localPoint = RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, UI.Camera,nil)
    local offsetMin = rectTransform.offsetMin
    local offsetMax = rectTransform.offsetMax
    if localPoint.x < offsetMin.x + margin[3] then 
        localPoint.x = offsetMin.x + margin[3]
    end
    if localPoint.y < offsetMin.y  + margin[2] then 
        localPoint.y = offsetMin.y + margin[2]
    end
    if localPoint.x > offsetMax.x  - margin[4] then 
        localPoint.x = offsetMax.x - margin[4]
    end
    if localPoint.y > offsetMax.y  - margin[1] then 
        localPoint.y = offsetMax.y - margin[1] 
    end
    return localPoint
end

--------------------------------------------------↑窗口逻辑↑--------------------------------------------------------