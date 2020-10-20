﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class GameAssetWrap
{
	public static void Register(LuaState L)
	{
		L.BeginStaticLibs("GameAsset");
		L.RegFunction("GetAssetBytes", GetAssetBytes);
		L.RegFunction("LoadSceneSingle", LoadSceneSingle);
		L.RegFunction("WaitLoadSceneSingle", WaitLoadSceneSingle);
		L.RegFunction("LoadSceneAdditive", LoadSceneAdditive);
		L.RegFunction("WaitLoadSceneAdditive", WaitLoadSceneAdditive);
		L.RegFunction("UnloadScene", UnloadScene);
		L.RegFunction("LoadLabel", LoadLabel);
		L.RegFunction("LoadAsync", LoadAsync);
		L.RegFunction("LoadAsset", LoadAsset);
		L.RegFunction("LoadUIModel", LoadUIModel);
		L.RegFunction("AddCacheUIModel", AddCacheUIModel);
		L.RegFunction("LoadObjects", LoadObjects);
		L.RegFunction("LoadAssets", LoadAssets);
		L.RegFunction("LoadPrefab", LoadPrefab);
		L.RegFunction("LoadSprite", LoadSprite);
		L.RegFunction("LoadTexture", LoadTexture);
		L.RegFunction("Release", Release);
		L.RegFunction("ReleaseSingle", ReleaseSingle);
		L.RegFunction("ReleaseAll", ReleaseAll);
		L.RegFunction("PrintCacheMap", PrintCacheMap);
		L.EndStaticLibs();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetAssetBytes(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			byte[] o = GameAsset.GetAssetBytes(arg0);
			ToLua.Push(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadSceneSingle(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			System.Action arg1 = (System.Action)ToLua.CheckDelegate<System.Action>(L, 2);
			GameAsset.LoadSceneSingle(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int WaitLoadSceneSingle(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			System.Action<float> arg1 = (System.Action<float>)ToLua.CheckDelegate<System.Action<float>>(L, 2);
			System.Collections.IEnumerator o = GameAsset.WaitLoadSceneSingle(arg0, arg1);
			ToLua.Push(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadSceneAdditive(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			System.Action arg1 = (System.Action)ToLua.CheckDelegate<System.Action>(L, 2);
			GameAsset.LoadSceneAdditive(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int WaitLoadSceneAdditive(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			System.Action<float> arg1 = (System.Action<float>)ToLua.CheckDelegate<System.Action<float>>(L, 2);
			System.Collections.IEnumerator o = GameAsset.WaitLoadSceneAdditive(arg0, arg1);
			ToLua.Push(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int UnloadScene(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			GameAsset.UnloadScene(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadLabel(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			System.Action<UnityEngine.Object> arg1 = (System.Action<UnityEngine.Object>)ToLua.CheckDelegate<System.Action<UnityEngine.Object>>(L, 2);
			GameAsset.LoadLabel(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadAsync(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			System.Action<UnityEngine.Object> arg1 = (System.Action<UnityEngine.Object>)ToLua.CheckDelegate<System.Action<UnityEngine.Object>>(L, 2);
			GameAsset.LoadAsync(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadAsset(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			System.Action<UnityEngine.Object> arg1 = (System.Action<UnityEngine.Object>)ToLua.CheckDelegate<System.Action<UnityEngine.Object>>(L, 2);
			GameAsset.LoadAsset(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadUIModel(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			System.Action<UnityEngine.GameObject> arg1 = (System.Action<UnityEngine.GameObject>)ToLua.CheckDelegate<System.Action<UnityEngine.GameObject>>(L, 2);
			GameAsset.LoadUIModel(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddCacheUIModel(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
			GameAsset.AddCacheUIModel(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadObjects(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string[] arg0 = ToLua.CheckStringArray(L, 1);
			System.Action<int,UnityEngine.Object> arg1 = (System.Action<int,UnityEngine.Object>)ToLua.CheckDelegate<System.Action<int,UnityEngine.Object>>(L, 2);
			GameAsset.LoadObjects(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadAssets(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string[] arg0 = ToLua.CheckStringArray(L, 1);
			System.Action<int,UnityEngine.Object> arg1 = (System.Action<int,UnityEngine.Object>)ToLua.CheckDelegate<System.Action<int,UnityEngine.Object>>(L, 2);
			System.Collections.IEnumerator o = GameAsset.LoadAssets(arg0, arg1);
			ToLua.Push(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadPrefab(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Action<UnityEngine.GameObject> arg1 = (System.Action<UnityEngine.GameObject>)ToLua.CheckDelegate<System.Action<UnityEngine.GameObject>>(L, 2);
				GameAsset.LoadPrefab(arg0, arg1);
				return 0;
			}
			else if (count == 3)
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Action<UnityEngine.GameObject> arg1 = (System.Action<UnityEngine.GameObject>)ToLua.CheckDelegate<System.Action<UnityEngine.GameObject>>(L, 2);
				bool arg2 = LuaDLL.luaL_checkboolean(L, 3);
				GameAsset.LoadPrefab(arg0, arg1, arg2);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: GameAsset.LoadPrefab");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadSprite(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Action<UnityEngine.Sprite> arg1 = (System.Action<UnityEngine.Sprite>)ToLua.CheckDelegate<System.Action<UnityEngine.Sprite>>(L, 2);
				GameAsset.LoadSprite(arg0, arg1);
				return 0;
			}
			else if (count == 3)
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Action<UnityEngine.Sprite> arg1 = (System.Action<UnityEngine.Sprite>)ToLua.CheckDelegate<System.Action<UnityEngine.Sprite>>(L, 2);
				bool arg2 = LuaDLL.luaL_checkboolean(L, 3);
				GameAsset.LoadSprite(arg0, arg1, arg2);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: GameAsset.LoadSprite");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadTexture(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Action<UnityEngine.Texture> arg1 = (System.Action<UnityEngine.Texture>)ToLua.CheckDelegate<System.Action<UnityEngine.Texture>>(L, 2);
				GameAsset.LoadTexture(arg0, arg1);
				return 0;
			}
			else if (count == 3)
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Action<UnityEngine.Texture> arg1 = (System.Action<UnityEngine.Texture>)ToLua.CheckDelegate<System.Action<UnityEngine.Texture>>(L, 2);
				bool arg2 = LuaDLL.luaL_checkboolean(L, 3);
				GameAsset.LoadTexture(arg0, arg1, arg2);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: GameAsset.LoadTexture");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Release(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			int arg1 = (int)LuaDLL.luaL_checknumber(L, 2);
			GameAsset.Release(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ReleaseSingle(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.CheckObject<UnityEngine.Object>(L, 1);
			GameAsset.ReleaseSingle(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ReleaseAll(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			GameAsset.ReleaseAll(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PrintCacheMap(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			GameAsset.PrintCacheMap();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

