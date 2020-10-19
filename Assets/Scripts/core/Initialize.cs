using Framework;
using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Initialize : MonoBehaviour
{
    void Start()
    {
        InitAppSetting();
        this.gameObject.AddComponent<GameManager>();

        GameAsset.LoadSceneSingle("Test", () =>
        {
            Debug.Log("GameAsset.LoadSceneSingle");
        });
    }
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