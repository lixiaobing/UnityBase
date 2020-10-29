using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneSocket
{
    public int LastStatus = 0;
    public SoketClient socket = null;
}

public class NetManager : LuaBase {

    private static NetManager Instance = null;
    
    private static readonly List<OneSocket> socketList = new List<OneSocket>();
    public static NetManager GetInstance()
    {
        return Instance;
    }
    protected override string getLuaName()
    {
        return this.GetType().Name;
    }
    public void RegisterSocket(SoketClient _socket)
    {
        socketList.Add(new OneSocket {
            LastStatus = 0,
            socket = _socket
        });
    }

    public void RemoveSocket(SoketClient _socket)
    {
        for (int i = 0; i < socketList.Count; i++)
        {
            if (socketList[i].socket == _socket)
            {
                socketList[i].socket = null;
                socketList.RemoveAt(i);
                break;
            }
        }
    }    

    public void SetLastStatus(SoketClient _socket ,int status)
    {
        foreach(var socket in socketList)
        {
            if (socket.socket == _socket)
            {
                socket.LastStatus = status;
            }
        }
    }
    
    protected override void initAwake()
    {
        DontDestroyOnLoad(this.gameObject); //禁止销毁
        Instance = this;
 
    }
    // Use this for initialization
    void Start () {


    }
	
	// Update is called once per frame
	void Update() {

        foreach (var target in socketList)
        {
            if (target.socket == null)
            {
                return;
            }

            if (target.LastStatus != (int)target.socket.GetSocketStatus())
            {
                target.socket.ExecuteSocketStatusEvent();
            }
            //取出队列的数据传给lua
            Queue<ByteBuffer> ReceiveQueue = target.socket.GetReceiveQueue();
            if (ReceiveQueue.Count > 0)
            {
                ByteBuffer GmaeByte = ReceiveQueue.Dequeue();
                this.CallGameByteBufferFunc("NetHelper.Receive", GmaeByte);
            }
            //取出发送队列直接发送
            Queue<ByteBuffer> WriteQueue = target.socket.GetWriteQueue();
            if (WriteQueue.Count > 0)
            {
                ByteBuffer GmaeByte = WriteQueue.Dequeue();
                target.socket.WriteMessage(GmaeByte);
            }
        }
        
    }
}
