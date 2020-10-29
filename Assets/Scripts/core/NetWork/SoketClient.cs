using Framework;
using LuaInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class SoketClient
{
    int PACKAGE_DECODE_KEYS_SIZE = 8;
    private static SoketClient instance = null;
    private const int MAX_READ = 1024 * 65;
    private byte[] byteBuffer = new byte[MAX_READ];
    private byte[] byteBufferTemp = null;
    private TcpClient client = null;
    private NetworkStream outStream = null;
    private Queue<ByteBuffer> ReceiveBytesQueue = null;
    private Queue<ByteBuffer> WriteBytesQueue = null;
    private SocketStatus status = SocketStatus.CLOSE;

    private System.Action<int> m_nConnHandle;

    public int[] m_szDefaultKeys;
    public int[] m_szServerKeys;
    public int[] m_szClientKeys;
    public int[] _defualtKeys;
    public short _headToken;
    public bool m_bUseDefaultKeys;

    enum SocketStatus
    {
        CLOSE = 1,
        READ_DATA = 2,
        WRITE_DATA = 3,
        CONNECT = 4,
        CONNECT_FAIL = 5,
        SERVER_CLOSE = 6
    };
    //
    public static SoketClient GetInstance()
    {
        if(instance == null)
        {
            instance = new SoketClient();
        }
        return instance;
    }
    public SoketClient()
    {
        ReceiveBytesQueue = new Queue<ByteBuffer>();
        WriteBytesQueue = new Queue<ByteBuffer>();

        Reset();

        NetManager.GetInstance().RegisterSocket(this);
    }

    private void Reset()
    {
        m_bUseDefaultKeys = true;
        _defualtKeys = new int[] { 0xac, 0x12, 0x19, 0xcd, 0x95, 0x34, 0xcb, 0xf1 };
        m_szDefaultKeys = _defualtKeys;

        SetHeadToken(0x712b);

        ReceiveBytesQueue.Clear();
        WriteBytesQueue.Clear();
    }

    public void SetHeadToken(short _h)
    {
        _headToken = _h;
    }
    public void SetEncodeKeys(int[] keys, bool bClient)
    {
        if (bClient)
            m_szServerKeys = keys;
        else
            m_szClientKeys = keys;
    }

    public int[] GetEncodeKeys(int nType)
    {
        if (m_bUseDefaultKeys == false)
        {
            if (nType == 0)//Client Encode KEY
                return m_szServerKeys;
            else if (nType == 1)//Client Decode KEY
                return m_szClientKeys;
        }
        else
            return m_szDefaultKeys;

        return null;
    }

    public void PacketEncode(byte[] byteData)
    {
        try
        {
            int length = byteData.Length;
            //byte[] P_byte_key = { 1, 2, 3, 4, 5, 6, 7, 8 };//Encoding.Unicode.GetBytes(encryptKey);//将密钥字符串转换为字节序列
            int[] P_byte_key = GetEncodeKeys(0);//{ 0xac, 0x12, 0x19, 0xcd, 0x95, 0x34, 0xcb, 0xf1 };
            byte lastCipherByte = 0;//(byte)((byteData[0] ^ P_byte_key[0]) & 0xff);

            int keyIndex = 0;
            for (int i = 0; i < length; i++)
            {
                keyIndex = (i & 0x7);
                P_byte_key[keyIndex] = (byte)(((P_byte_key[keyIndex] + lastCipherByte) ^ i) & 0xff);
                lastCipherByte = (byte)((((byteData[i] ^ P_byte_key[keyIndex]) & 0xff) + lastCipherByte) & 0xff);
                byteData[i] = lastCipherByte;
            }

            if (m_bUseDefaultKeys)
                m_bUseDefaultKeys = false;
        }
        catch (CryptographicException ce)
        {
            throw new Exception(ce.Message);
        }
    }

    public void PacketDecode(byte[] bytes,int start ,int length)
    {
        int[] pKeys = GetEncodeKeys(1);
        byte lastCipherByte = 0;
        int plainText;
        int index;
        int key;
        int keyIndex = 0;
        for(index = start; index < length;index++)
        {
            key = ((pKeys[keyIndex & 0x7] + lastCipherByte) ^ keyIndex);
            plainText = ((((bytes[index] & 0xff) - lastCipherByte) ^ key) & 0xff);

            lastCipherByte = (byte)(bytes[index] & 0xff);
            bytes[index] = (byte)plainText;
            pKeys[keyIndex & 0x7] = key & 0xff;
            keyIndex += 1;
        }
    }

    //写一个数据到队列
    public void WriteGameByteBuffer(ByteBuffer ByteBuffer)
    {
        PacketEncode(ByteBuffer.GetBytes());
        WriteBytesQueue.Enqueue(ByteBuffer);
    }
    //获取读取队列
    public Queue<ByteBuffer> GetReceiveQueue()
    {
        return ReceiveBytesQueue;
    }
    //获取写队列
    public Queue<ByteBuffer> GetWriteQueue()
    {
        return WriteBytesQueue;
    }
    public int GetSocketStatus()
    {
        return (int)status;
    }
    //连接到服务器
    public void ConnectServer(string host, int port, System.Action<int> nConnHandle)
    {
        if (client != null)
            Close();

        try
        {
            status = SocketStatus.CLOSE;
            NetManager.GetInstance().SetLastStatus(this,(int)status);

            IPAddress[] address = Dns.GetHostAddresses(host);
            if (address.Length == 0)
            {
                Debug.LogError("host invalid");
                return;
            }
            if (address[0].AddressFamily == AddressFamily.InterNetworkV6)
            {
                client = new TcpClient(AddressFamily.InterNetworkV6);
            }
            else
            {
                client = new TcpClient(AddressFamily.InterNetwork);
            }
            client.SendTimeout = 10000;
            client.ReceiveTimeout = 10000;
            client.NoDelay = true;
            client.BeginConnect(host, port, new AsyncCallback(OnConnect), client);
            m_nConnHandle = nConnHandle;

            //TcpListener
        }
        catch (Exception e)
        {
            //status = SocketStatus.CLOSE;
            Close();
            Debug.LogError(e.Message);
        }

    }
    /// <summary>
    /// 连接上服务器
    /// </summary>
    void OnConnect(IAsyncResult asr)
    {   
        try
        {
            //TcpClient tcpclient = asr.AsyncState as TcpClient;
            if (client.Connected)
            {
                client.EndConnect(asr);
                outStream = client.GetStream();
                client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), outStream);
                status = SocketStatus.CONNECT;
                //ExecuteSocketStatusEvent();
            }
            else
            {
                
                status = SocketStatus.CONNECT_FAIL;
                Close();
                //ExecuteSocketStatusEvent();
            }
            
        }
        catch(Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        
    }

    public bool IsOnline()
    {
        return (client != null) && (!((client.Client.Poll(1000, SelectMode.SelectRead) && (client.Client.Available == 0)) || !client.Client.Connected));
    }

    public void ExecuteSocketStatusEvent()
    {
        if (m_nConnHandle == null)
            return;

        m_nConnHandle((int)status);
        NetManager.GetInstance().SetLastStatus(this,(int)status);
    }

    public void Disconnect()
    {
        Close();
        status = SocketStatus.CLOSE;
        ExecuteSocketStatusEvent();
    }

    public void Destory()
    {
        Disconnect();
        NetManager.GetInstance().RemoveSocket(this);
    }

    /// <summary>
    /// 关闭链接
    /// </summary>
    public void Close()
    {
        if (byteBufferTemp != null)
        {
            Array.Clear(byteBufferTemp, 0, byteBufferTemp.Length);
            byteBufferTemp = null;
        }

        if (client != null)
        {
            if (client.Connected)
            {
                client.GetStream().Flush();
                client.GetStream().Close();
                outStream = null;
                client.Close();
                client.Dispose();
                client = null;
            }
        }
        Reset();
    }
    /// <summary>
    /// 读取消息
    /// </summary>
    void OnRead(IAsyncResult asr)
    {
        int bytesRead = 0;
        try
        {
            //status = SocketStatus.READ_DATA;

            if (client == null)
            {
                return;
            }

            if (client.Connected == false)
            {
                return;
            }

            lock (client.GetStream())
            {         //读取字节流到缓冲区
                bytesRead = client.GetStream().EndRead(asr);
            }
            if (bytesRead < 1)
            {
                //包尺寸有问题，断线处理
                status = SocketStatus.READ_DATA;
                Close();
                //ExecuteSocketStatusEvent();
                return;
            }

            //分析数据包内容，抛给逻辑层
            if (byteBufferTemp != null && byteBufferTemp.Length > 0)
            {
                byte[] bytes = new byte[byteBuffer.Length + byteBufferTemp.Length];
                Array.Copy(byteBufferTemp, 0, bytes, 0, byteBufferTemp.Length);
                Array.Copy(byteBuffer, 0, bytes, byteBufferTemp.Length, byteBuffer.Length);
                bytesRead += byteBufferTemp.Length;
                Array.Clear(byteBufferTemp, 0, byteBufferTemp.Length);
                byteBufferTemp = null;

                OnReceive(bytes, bytesRead);
            }
            else
            {
                OnReceive(byteBuffer, bytesRead);
            }
                
            lock (client.GetStream())
            {    
                //分析完，再次监听服务器发过来的新消息
                Array.Clear(byteBuffer, 0, byteBuffer.Length);   //清空数组
                client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), outStream);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            status = SocketStatus.SERVER_CLOSE;
            Close();
            //ExecuteSocketStatusEvent();
        }
    }


    int GetPacketLen(byte[] bytes,int start)
    {
        int[] pKeys = GetEncodeKeys(1);
        byte plainByte1;
        int key;
        byte plainByte2;

        int cipherByte1 = bytes[start] & 0xff;
        key = pKeys[0];
        plainByte1 = (byte)(((cipherByte1 ^ key) & 0xff));

        int cipherByte2 = bytes[start + 1] & 0xff;
        key = (int)(((pKeys[1] + cipherByte1) ^ 1) & 0xff);
        plainByte2 = (byte)(((cipherByte2 - cipherByte1) ^ key) & 0xff);

        byte[] headbyte = new byte[]{ plainByte1, plainByte2 };
        Array.Reverse(headbyte);
        int headLen = BitConverter.ToInt16(headbyte, 0);

        int cipherByte3 = bytes[start+2] & 0xff;
        key = ((pKeys[2] + cipherByte2) ^ 2) & 0xff;
        byte plainByte3 = (byte)(((cipherByte3 - cipherByte2) ^ key) & 0xff);

        int cipherByte4 = bytes[start+3] & 0xff;
        key = ((pKeys[3] + cipherByte3) ^ 3) & 0xff;
        byte plainByte4 = (byte)(((cipherByte4 - cipherByte3) ^ key) & 0xff);

        int cipherByte5 = bytes[start+4] & 0xff;
        key = ((pKeys[4] + cipherByte4) ^ 4) & 0xff;
        byte plainByte5 = (byte)(((cipherByte5 - cipherByte4) ^ key) & 0xff);

        int cipherByte6 = bytes[start+5] & 0xff;
        key = ((pKeys[5] + cipherByte5) ^ 5) & 0xff;
        byte plainByte6 = (byte)(((cipherByte6 - cipherByte5) ^ key) & 0xff);

        byte[] lenbyte = new byte[] { plainByte3, plainByte4, plainByte5, plainByte6 };
        Array.Reverse(lenbyte);
        int len = BitConverter.ToInt32(lenbyte, 0);

        return len;
    }

    //读取数据
    void OnReceive(byte[] bytes, int length)
    {
        // Array.Reverse(bytes, 0, length);

        //拆包处理分包 |----2----|------4------|----2----|----4----|----4----|-------------	
        //                 FLAG    length       checkSum   msgId      status       data
        int resolveLen = 0;
        while (resolveLen<length)
        {
            int packlen = GetPacketLen(bytes, resolveLen);

            //判断数据是否完整，否则等接收完整再处理
            int leftoverLength = length - resolveLen;
            if (packlen > leftoverLength)
            {
                byteBufferTemp = new byte[leftoverLength];
                Array.Copy(bytes, resolveLen, byteBufferTemp, 0, leftoverLength);
                break;
            }

            //解密
            PacketDecode(bytes, resolveLen, resolveLen + packlen);

            byte[] headLen = new byte[2];
            Array.Copy(bytes, resolveLen, headLen, 0, headLen.Length);
            Array.Reverse(headLen);
            int nHeadLen = BitConverter.ToInt16(headLen, 0);

            //数据段大小
            resolveLen += 2;
            byte[] sizeBytes = new byte[4];
            Array.Copy(bytes, resolveLen, sizeBytes, 0, sizeBytes.Length);
            Array.Reverse(sizeBytes);
            int size = BitConverter.ToInt32(sizeBytes, 0);

            resolveLen += 4;
            byte[] checksumBytes = new byte[2];
            Array.Copy(bytes, resolveLen, checksumBytes, 0, checksumBytes.Length);
            Array.Reverse(checksumBytes);
            int checkSum = BitConverter.ToInt16(checksumBytes, 0);

            //protoId
            resolveLen += 2;
            byte[] protoidBytes = new byte[4];
            Array.Copy(bytes, resolveLen, protoidBytes, 0, protoidBytes.Length);
            Array.Reverse(protoidBytes);
            int protoId = BitConverter.ToInt32(protoidBytes, 0);

            //errorcode
            resolveLen += 4;
            byte[] errorCodeBytes = new byte[4];
            Array.Copy(bytes, resolveLen, errorCodeBytes, 0, errorCodeBytes.Length);
            Array.Reverse(errorCodeBytes);
            int errorCode = BitConverter.ToInt32(errorCodeBytes, 0);

            resolveLen += 4;
            int prptoDataSize = size - 16;
            //数据
            ByteBuffer ByteBuffer = ByteBuffer.getNewInstance();
            ByteBuffer.ParseByteBuffer(ref bytes, resolveLen, prptoDataSize, protoId, errorCode,this);
            ReceiveBytesQueue.Enqueue(ByteBuffer);
            resolveLen += prptoDataSize;
        }
    }
    //
    public void WriteMessage(ByteBuffer ByteBuffer)
    { 
        if (client != null && client.Connected)
        {
            byte[] payload = ByteBuffer.GetBytes();
            outStream.BeginWrite(payload, 0, payload.Length, new AsyncCallback(OnWrite), null);
        }
        else
        {
            Debug.LogError("client.connected----->>false");
        }
    }
    void OnWrite(IAsyncResult r)
    {
        try
        {
            outStream.EndWrite(r);
        }
        catch (Exception ex)
        {
            Debug.LogError("OnWrite--->>>" + ex.Message);
        }
    }
}
