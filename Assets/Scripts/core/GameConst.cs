using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public class GameConst
    {
        //应用名
        public const string AppName = "DAL2";

        //游戏帧率
        public const int FrameRate = 60;

        //开发期调试模式
        public static bool DebugMode = true;

        //移动平台Lua文件的bytecode模式
        public const bool LuaByteMode = false;



        //====================以下废弃 暂时不再使用======================================

        //打包资源文件
        public static bool ResourcesBundleMode = false;
        //移动平台Lua文件的assetBundle模式
        public const bool LuaBundleMode = true;
        //Lua临时存放目录
        public const string LuaTempFolder = "LuaTemp/";                    //临时目录
        //Bundle目录
        public const string BundleFolder = "StreamingAssets";
        //Bundle文件后缀
        public const string BundleExtName = ".unity3d";
        //热更新
        public const bool UpdateMode = false;
        //热更新URL
        public const string UpdateURL = "http://localhost:6688/";
    }
}
