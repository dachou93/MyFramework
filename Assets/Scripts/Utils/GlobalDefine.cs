using System;
using UnityEngine;

/// <summary>
/// 全局变量控制类
/// </summary>
public static class GlobalDefine
{
    /// <summary>
    /// 资源默认路径
    /// </summary>
    public static string DataPath { get; } = Environment.CurrentDirectory + "/RawData";
  
}