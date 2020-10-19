--主入口函数。从这里开始lua逻辑
require "init"

function OnStart()					
	print("Lua OnStart")	
	SceneManager.sceneLoaded = SceneManager.sceneLoaded + OnSceneLoaded 		
end

--场景切换通知
function OnSceneLoaded(level)
	print("Lua OnSceneLoaded")	
	collectgarbage("collect")
	Time.timeSinceLevelLoad = 0
end


function OnDestroy()
	print("Lua OnDestroy")	
	SceneManager.sceneLoaded = SceneManager.sceneLoaded - OnSceneLoaded 		
end