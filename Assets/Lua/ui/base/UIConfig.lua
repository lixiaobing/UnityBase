UIConfig = {}
local this = UIConfig

UILayerNames = {
    UI = "UI",
    UIBg = "UIBg",
    UIBlack = "UIBlack",
    UIModel = "UIModel"
}

local LayerType = {
    Pool = 1, -- 缓存池
    Main = 2, -- 常驻UI
    Screen = 3, -- 全屏窗口
    ScreenUp = 4, -- 全屏窗口(公用)
    Window = 5, -- 常规窗口
    Popup1 = 6, -- 一级弹窗
    Popup2 = 7, -- 二级弹窗
    Popup3 = 8, -- 三级弹窗
    Popup4 = 9, -- 四级弹窗
    Popup5 = 10, -- 五级弹窗
    Guide1 = 11, -- 引导层
    Guide2 = 12, -- 引导时弹出的UI
    Loading = 13, -- 转场加载
    Notify = 14, -- 二次确认弹窗
    Hints = 15, -- 小提示
    Lock = 16, -- 锁定层
    Dark = 17, -- 黑屏层
    Popup = 99 -- 弹窗标识
}

local Layers = {
    "pool",
    "main",
    "screen",
    "screenup",
    "window",
    "popup1",
    "popup2",
    "popup3",
    "popup4",
    "popup5",
    "guide1",
    "guide2",
    "loading",
    "notify",
    "hints",
    "lock",
    "dark"
}

local BgConfig = {
}

local CoverLayers = {6, 7, 8, 9, 10, 11, 13, 14}
local CoverDefaultColor = Color(0, 0, 0, 0.8)
local CoverColors = {
    BattlePauseWindow = Color(0, 0, 0, 0.95)
}

local Settings = {}

UIConfig.LayerType = LayerType
UIConfig.Layers = Layers
UIConfig.BgConfig = BgConfig
UIConfig.CoverLayers = CoverLayers
UIConfig.CoverDefaultColor = CoverDefaultColor
UIConfig.CoverColors = CoverColors
UIConfig.Settings = Settings

function UIConfig.getLayerTypeByName(name)
    for i, v in ipairs(Layers) do
        if v == name then
            return i
        end
    end
    return 0
end

--------------------------------------
-- @FuncName：add_setting
-- @Function：添加界面关系映射表
-- @param：window_name   窗口名字
-- @param：window_class  窗口类名
-- @param：window_path  窗口预制体所在路径
-- @param：window_layer  窗口所在层
-- @param：is_cache    窗口是否被缓存
--------------------------------------
local addSetting = function(window_name, window_class, window_path, window_layer, is_cache)
    Settings[window_name] = {}

    local set = Settings[window_name]
    --UI Prefab名称
    set.name = window_name
    --UI控制类
    set.class = window_class
    --Bundle名称
    set.path = window_path
    --所在层次
    set.layer = window_layer
    --是否缓存UI
    set.cache = is_cache
end

local initSetting = function()

    addSetting("MainWindow", MainWindow, "UI/Window/MainWindow", UIConfig.LayerType.Screen, false)
   
end

function UIConfig.init()
    initSetting()
    UIManager.init()
end
