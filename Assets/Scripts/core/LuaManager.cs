
using UnityEngine;
using System.Collections.Generic;
using LuaInterface;
using System.Collections;
using System.IO;
using System;
#if UNITY_5_4_OR_NEWER
using UnityEngine.SceneManagement;
#endif

namespace Framework
{
    public class LuaManager : MonoBehaviour
    {
        public static LuaManager Instance
        {
            get;
            protected set;
        }

        protected LuaState luaState = null;
        protected LuaLoader loader;
        protected LuaLooper loop = null;
        protected bool openLuaSocket = false;
        protected bool beZbStart = false;

        protected virtual void OpenLibs()
        {
            luaState.OpenLibs(LuaDLL.luaopen_pb);
            luaState.OpenLibs(LuaDLL.luaopen_struct);
            luaState.OpenLibs(LuaDLL.luaopen_lpeg);
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        luaState.OpenLibs(LuaDLL.luaopen_bit);
#endif

            OpenCJson();

            if (LuaConst.openLuaSocket)
            {
                OpenLuaSocket();
            }

            if (LuaConst.openLuaDebugger)
            {
                OpenZbsDebugger();
            }
        }

        public void OpenZbsDebugger(string ip = "localhost")
        {
            if (!Directory.Exists(LuaConst.zbsDir))
            {
                Debugger.LogWarning("ZeroBraneStudio not install or LuaConst.zbsDir not right");
                return;
            }

            if (!LuaConst.openLuaSocket)
            {
                OpenLuaSocket();
            }

            if (!string.IsNullOrEmpty(LuaConst.zbsDir))
            {
                luaState.AddSearchPath(LuaConst.zbsDir);
            }

            luaState.LuaDoString(string.Format("DebugServerIp = '{0}'", ip), "@LuaClient.cs");
        }


  

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int LuaOpen_Socket_Core(IntPtr L)
        {
            return LuaDLL.luaopen_socket_core(L);
        }

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int LuaOpen_Mime_Core(IntPtr L)
        {
            return LuaDLL.luaopen_mime_core(L);
        }

        protected void OpenLuaSocket()
        {
            LuaConst.openLuaSocket = true;

            luaState.BeginPreLoad();
            luaState.RegFunction("socket.core", LuaOpen_Socket_Core);
            luaState.RegFunction("mime.core", LuaOpen_Mime_Core);
            luaState.EndPreLoad();
        }

        //cjson 比较特殊，只new了一个table，没有注册库，这里注册一下
        protected void OpenCJson()
        {
            luaState.LuaGetField(LuaIndexes.LUA_REGISTRYINDEX, "_LOADED");
            luaState.OpenLibs(LuaDLL.luaopen_cjson);
            luaState.LuaSetField(-2, "cjson");

            luaState.OpenLibs(LuaDLL.luaopen_cjson_safe);
            luaState.LuaSetField(-2, "cjson.safe");
        }
        protected virtual void StartMain()
        {
            luaState.DoFile("main.lua");
            LuaFunction onStart = luaState.GetFunction("OnStart");
            onStart.Call();
            onStart.Dispose();

        }

        protected void StartLooper()
        {
            loop = gameObject.AddComponent<LuaLooper>();
            loop.luaState = luaState;
        }

        protected virtual void Bind()
        {
            LuaBinder.Bind(luaState);
            DelegateFactory.Init();
            LuaCoroutine.Register(luaState, this);
        }

        public void Init()
        {
            loader = new LuaLoader();
            luaState = new LuaState();
            OpenLibs();
            luaState.LuaSetTop(0);
            Bind();

            //SceneManager.sceneLoaded += OnSceneLoaded;

            luaState.Start();

            StartLooper();
            StartMain();
        }

/*        void OnSceneLoaded(Scene scene, LoadSceneMode mode) {

            if (luaState != null)
            {
                luaState.RefreshDelegateMap();
            }
        }*/


        public virtual void Destroy()
        {
            if (luaState != null)
            {
                //SceneManager.sceneLoaded -= OnSceneLoaded;
                luaState.Call("OnDestroy", false);
                DetachProfiler();
                LuaState state = luaState;
                luaState = null;
                if (loop != null)
                {
                    loop.Destroy();
                    loop = null;
                }

                state.Dispose();
                Instance = null;
            }
        }

        protected void OnDestroy()
        {
            Destroy();
        }

        protected void OnApplicationQuit()
        {
            Destroy();
        }

        public static LuaState GetMainState()
        {
            return Instance.luaState;
        }

        public LuaLooper GetLooper()
        {
            return loop;
        }

        LuaTable profiler = null;

        public void AttachProfiler()
        {
            if (profiler == null)
            {
                profiler = luaState.Require<LuaTable>("UnityEngine.Profiler");
                profiler.Call("start", profiler);
            }
        }
        public void DetachProfiler()
        {
            if (profiler != null)
            {
                profiler.Call("stop", profiler);
                profiler.Dispose();
                LuaProfiler.Clear();
            }
        }



        public void DoLuaString(string text)
        {
            luaState.DoString(text);
        }
        public void DoLuaFile(string file)
        {
            this.luaState.DoFile(file);
        }
        public LuaState GetLuaState()
        {
            return luaState;
        }
        public object[] CallFunction(string funcName, params object[] args)
        {
            LuaFunction func = luaState.GetFunction(funcName);
            if (func != null)
            {
                return func.Invoke<object[], object[]>(args);
            }
            return null;
        }

        public LuaFunction CallUpdateFunc(string func, GameObject obj, ref LuaFunction call)
        {

            LuaFunction luaFunc = null;
            if (call != null)
            {
                luaFunc = call;
            }
            else
            {
                luaFunc = luaState.GetFunction(func);
                if (luaFunc == null)
                {
                    Debug.LogError("!!!Lua无法获取这个方法,请检查！Function Name:" + func);
                    return null;
                }
            }
            luaFunc.Call(obj);
            return luaFunc;

        }


        /*        public void CallGameByteBufferFunc(string func, TFByteBuffer param)
                {
                    LuaFunction luaParamFunc = luaState.GetFunction(func);
                    if (luaParamFunc == null)
                    {
                        Debug.LogError("!!!Lua无法获取这个方法,请检查！Function Name:" + func);
                        return;
                    }
                    luaParamFunc.BeginPCall();
                    luaParamFunc.Push(param);
                    luaParamFunc.PCall();
                    luaParamFunc.EndPCall();
                    luaParamFunc.Dispose();
                    luaParamFunc = null;
                }*/
    }

}