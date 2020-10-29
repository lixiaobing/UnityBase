using LuaInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class ByteBuffer {
    private int protoId = 0;
    private int errorCode = 0;
    private byte[] byteData = null;
    private SoketClient socket = null;

    public static ByteBuffer getNewInstance()
    {
        return new ByteBuffer();
    }
    public ByteBuffer()
    {
        
    }

    //读写分别代表发送和接收
    public void ParseByteBuffer(ref byte[] data,int copyOffis,int Len,int protoId,int errorCode, SoketClient _socket)
    {
        if (data != null)
        {
            byteData = new byte[Len];
            Array.Copy(data, copyOffis, byteData, 0, Len);
        }
        this.protoId = protoId;
        this.errorCode = errorCode;
        this.socket = _socket;
    }
    //获取字节数据
    public byte[] GetBytes()
    {
        return byteData;
    }
    //获取protoId
    public int GetProtoId()
    {
        return this.protoId;
    }

    public SoketClient GetSocket()
    {
        return this.socket;
    }
    public int GetErrorCode()
    {
        return this.errorCode;
    }

    //获取把byte转换成lua可用的
    public LuaByteBuffer ReadBuffer()
    {
        byte[] bytes = GetBytes();
        return new LuaByteBuffer(bytes);
    }

    public short CalcChecksum(byte[] pData, int _start,int _end)
    {

        int val = 0x77;
        int i = _start;
        int size = _end;
	    while (i<size)
	    {
		    val += (pData[i++] & 0xFF);
	    }
	    return (short)(val & 0x7F7F);
    }

    //写数据
    public void WriteBuffer(int luaProtoId, LuaByteBuffer strBuffer)
    {
        this.protoId = luaProtoId;
        byte[] data = strBuffer.buffer;
        int Len = data.Length;
        byteData = new byte[Len + 12];

        byte[] flag = BitConverter.GetBytes((short)0x712b);
        Array.Reverse(flag);
        Array.Copy(flag, 0, byteData, 0, flag.Length);

        byte[] contentLength = BitConverter.GetBytes(Len+12);
        Array.Reverse(contentLength);
        Array.Copy(contentLength, 0, byteData, 2, contentLength.Length);

        byte[] protoIdBytes = BitConverter.GetBytes(luaProtoId);
        Array.Reverse(protoIdBytes);
        Array.Copy(protoIdBytes, 0, byteData, 8, protoIdBytes.Length);
        Array.Copy(data, 0, byteData, 12, Len);

        short checksum = CalcChecksum(byteData,8,Len+12);
        byte[] checksum_byte = BitConverter.GetBytes(checksum);
        Array.Reverse(checksum_byte);
        Array.Copy(checksum_byte, 0, byteData, 6, checksum_byte.Length);

        Debug.Log(BitConverter.ToString(byteData));
    }
}
