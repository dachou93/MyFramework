using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MyStream : FileStream
{
    const byte KEY = 64;
    private int myoffset;
    public MyStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync,int myOffset) : base(path, mode, access, share, bufferSize, useAsync)
    {
        this.myoffset = myOffset;
       
    }
    public MyStream(string path, FileMode mode) : base(path, mode)
    {
    }
    public override int Read(byte[] array, int offset, int count)
    {
        
        var index = base.Read(array, offset, count);
        //for (int i = 0; i < array.Length; i++)
        //{
        //    array[i] ^= KEY;
        //}
        return index;
    }
    public override void Write(byte[] array, int offset, int count)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] ^= KEY;
        }
        base.Write(array, offset, count);
    }
    //public override long Seek(long offset, SeekOrigin origin)
    //{
    //    Debug.Log(offset);
    //    return base.Seek(offset, origin);
    //}

}
