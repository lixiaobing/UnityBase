using Framework;
using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public sealed class App : MonoBehaviour
{

    public static App Instance
    {
        get;
        private set;
    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        Instance = this;
    }
    void Start()
    {
        InitAppSetting();
        this.gameObject.AddComponent<GameManager>();

    }

/*    public static Coroutine Start(IEnumerator func)
    {
        return Instance.StartCoroutine(func);
    }
  
    public static void StopAll(IEnumerator func)
    {
        Instance.StopAllCoroutines();
    }

    public static void StopOne(Coroutine task)
    {
        Instance.StopCoroutine(task);
    }*/




    //初始化屏幕适配
    void InitAppSetting()
    {
        Screen.orientation = ScreenOrientation.Landscape;
        //Screen.fullScreen = true;
        //Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = GameConst.FrameRate;

        if (Application.isMobilePlatform)
        {
            GameConst.DebugMode = false;

        }
    }
}