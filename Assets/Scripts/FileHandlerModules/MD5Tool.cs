using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class MD5Tool
{
    public static string GetMD5ByPathUseBytes(string path)
    {
        FileStream fs = new FileStream(path, FileMode.Open);
        BinaryReader r = new BinaryReader(fs);
        byte[] bytes = r.ReadBytes((int)fs.Length);//单个模型文件最大不能超过2g
        fs.Close();
        r.Close();
        string md5 = MD5Tool.MD5Byte(bytes);
        return md5;
    }

    public static string GetMD5ByPathUseBytes(string path,out byte[] bytes)
    {
        FileStream fs = new FileStream(path, FileMode.Open);
        BinaryReader r = new BinaryReader(fs);
        bytes = r.ReadBytes((int)fs.Length);//单个模型文件最大不能超过2g
        fs.Close();
        r.Close();
        string md5 = MD5Tool.MD5Byte(bytes);
        return md5;
    }

    public static string GetFileMD5(string filepath)
    {
        FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
        int bufferSize = 1048576;

        byte[] buff = new byte[bufferSize];
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        md5.Initialize();
        long offset = 0;
        while (offset < fs.Length)
        {
            long readSize = bufferSize;
            if (offset + readSize > fs.Length)
                readSize = fs.Length - offset;
            fs.Read(buff, 0, Convert.ToInt32(readSize));
            if (offset + readSize < fs.Length)
                md5.TransformBlock(buff, 0, Convert.ToInt32(readSize), buff, 0);
            else
                md5.TransformFinalBlock(buff, 0, Convert.ToInt32(readSize));
            offset += bufferSize;
        }
        if (offset >= fs.Length)
        {
            fs.Close();
            byte[] result = md5.Hash;
            md5.Clear();
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < result.Length; i++)
                sb.Append(result[i].ToString("X2"));
            return sb.ToString();
        }
        else
        {
            fs.Close();
            return null;
        }
    }
    /// <summary>
    /// 对文件流进行MD5加密
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    /// <example></example>
    public static string MD5Stream(Stream fs)
    {
        //FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        md5.ComputeHash(fs);
        fs.Close();

        byte[] b = md5.Hash;
        md5.Clear();

        StringBuilder sb = new StringBuilder(32);
        for (int i = 0; i < b.Length; i++)
        {
            sb.Append(b[i].ToString("X2"));
        }

        return sb.ToString();
    }


    /// <summary>
    /// 对文件流进行MD5加密
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    /// <example></example>
    public static string MD5Stream(string filePath)
    {
        FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        md5.ComputeHash(fs);
        fs.Close();

        byte[] b = md5.Hash;
        md5.Clear();

        StringBuilder sb = new StringBuilder(32);
        for (int i = 0; i < b.Length; i++)
        {
            sb.Append(b[i].ToString("X2"));
        }

        return sb.ToString();
    }

    /// <summary>
    /// 对文件进行MD5加密
    /// </summary>
    /// <param name="filePath"></param>
    public static void MD5File(string filePath)
    {
        FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        int bufferSize = 1048576; // 缓冲区大小，1MB
        byte[] buff = new byte[bufferSize];

        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        md5.Initialize();

        long offset = 0;
        while (offset < fs.Length)
        {
            long readSize = bufferSize;
            if (offset + readSize > fs.Length)
            {
                readSize = fs.Length - offset;
            }

            fs.Read(buff, 0, Convert.ToInt32(readSize)); // 读取一段数据到缓冲区

            if (offset + readSize < fs.Length) // 不是最后一块
            {
                md5.TransformBlock(buff, 0, Convert.ToInt32(readSize), buff, 0);
            }
            else // 最后一块
            {
                md5.TransformFinalBlock(buff, 0, Convert.ToInt32(readSize));
            }

            offset += bufferSize;
        }

        fs.Close();
        byte[] result = md5.Hash;
        md5.Clear();

        StringBuilder sb = new StringBuilder(32);
        for (int i = 0; i < result.Length; i++)
        {
            sb.Append(result[i].ToString("X2"));
        }
    }


    public static string MD5Byte(byte[] fileByte)
    {

        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        md5.ComputeHash(fileByte);

        byte[] b = md5.Hash;
        md5.Clear();

        StringBuilder sb = new StringBuilder(32);
        for (int i = 0; i < b.Length; i++)
        {
            sb.Append(b[i].ToString("X2"));
        }

        return sb.ToString();
    }
}
