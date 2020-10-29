using Framework;
using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuaBase : MonoBehaviour {
    private LuaManager luaManager = null;
    private LuaFunction luaUpdateFunc = null;
    //lua相关的一些处理
    protected string luaPath = "";
    protected string luaName = "";
    protected string luaInit = "";
    private string luaStart = "";
    private string luaUpdate = "";
    private string luaFile = "";
    public string START = ".Start";
    public string UPDATE = ".Update";
    public string INIT = ".Init";
    public string DESTROY = ".Destroy";
    //记录上次调用的名字
    private string upCallName = "";
    void Awake()
    {
        this.luaManager = GameManager.Instance.LuaMgr;
        this.luaName = this.getLuaName();
        this.initCallName();
        this.initAwake();
    }
    protected virtual void initAwake(){} 
    //需要子类重写
    protected virtual string getLuaName()
    {
        return "";
    }
    private void initCallName()
    {
        if(this.luaManager == null)
        {
            return;
        }
        this.luaStart = string.Concat(this.luaName, START);
        this.luaUpdate = string.Concat(this.luaName, UPDATE);
        this.luaInit = string.Concat(this.luaName, INIT);
        //string _file = "net/" + this.luaName + ".lua";
        //this.luaManager.doLuaFile(_file);
    }
    protected void callStart(GameObject obj)
    {
        if (this.luaManager == null)
        {
            return;
        }
        this.luaManager.CallFunction(this.luaStart, obj);
    }
    protected void callInit(GameObject obj)
    {
        if (this.luaManager == null)
        {
            return;
        }
        this.luaManager.CallFunction(this.luaInit, obj);
    }
    protected void testCallInit(GameObject obj)
    {
        if (this.luaManager == null)
        {
            return;
        }
        this.luaManager.CallFunction("Init", obj);
    }
    protected void callUpdate(GameObject obj)
    {
        if (this.luaManager == null)
        {
            return;
        }
        if (luaUpdateFunc == null)
        {
            luaUpdateFunc = this.luaManager.CallUpdateFunc(this.luaUpdate, obj,ref luaUpdateFunc);
        }
        else
        {
            this.luaManager.CallUpdateFunc(this.luaUpdate, obj,ref luaUpdateFunc);
        }
    }
    public void CallGameByteBufferFunc(string func, ByteBuffer param)
    {
        //this.luaManager.CallGameByteBufferFunc(func, param);
    }
    public void loadLuaFile(string name)
    {
        //this.luaManager.doLuaFile(name);
    }

}
