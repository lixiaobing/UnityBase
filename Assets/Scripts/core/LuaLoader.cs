using UnityEngine;
using System.Collections;
using System.IO;
using LuaInterface;
using System;

namespace Framework
{
    /// <summary>
    /// 集成自LuaFileUtils，重写里面的ReadFile，
    /// </summary>
    public class LuaLoader : LuaFileUtils
    {

        // Use this for initialization
        public LuaLoader()
        {
            instance = this;
        }

        /// <summary>
        /// 当LuaVM加载Lua文件的时候，这里就会被调用，
        /// 用户可以自定义加载行为，只要返回byte[]即可。
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override byte[] ReadFile(string fileName)
        {
            if (!fileName.EndsWith(".lua"))
            {
                fileName = fileName.Replace(".", "/");
                fileName += ".lua";
            }

            byte[] bytes = null;
            if (GameConst.DebugMode)
            {
                string luaDir = LuaConst.luaDir;
                string fullPath = luaDir + fileName;
                if (File.Exists(fullPath))
                {
                    return File.ReadAllBytes(fullPath);
                }
                string toLuaDir = LuaConst.toluaDir;
                fullPath = toLuaDir + fileName;
                if (File.Exists(fullPath))
                {
                    return File.ReadAllBytes(fullPath);
                }
            }
            else
            {
                fileName = "Lua/" + fileName.Replace(".lua","");
                if (ResManager.Instance.luaAssets.TryGetValue(fileName, out TextAsset asset))
                {
                    return asset.bytes;
                }
            }
            return bytes;
        }

    }
}