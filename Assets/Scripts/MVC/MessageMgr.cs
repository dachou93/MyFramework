using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 消息管理器
/// </summary>
public class MessageMgr : Singleton<MessageMgr>
{
    readonly Dictionary<string, List<object>> listenersMap = new Dictionary<string, List<object>>();

    private MessageMgr()
    {
        SceneManager.sceneUnloaded += (Scene scene) =>
        {
            listenersMap.Clear();
        };
    }

    public void AddListener(string msg, System.Action linstener)
    {
        if (!listenersMap.TryGetValue(msg, out List<object> linsteners))
        {
            linsteners = new List<object>();
            listenersMap.Add(msg, linsteners);
        }

        linsteners.Add(linstener);
    }

    public void AddListener(string msg, Delegate linstener)
    {
        if (!listenersMap.TryGetValue(msg, out List<object> linsteners))
        {
            linsteners = new List<object>();
            listenersMap.Add(msg, linsteners);
        }

        linsteners.Add(linstener);
    }

    public void AddListener<T>(string msg, System.Action<T> linstener)
    {
        if (!listenersMap.TryGetValue(msg, out List<object> linsteners))
        {
            linsteners = new List<object>();
            listenersMap.Add(msg, linsteners);
        }

        linsteners.Add(linstener);
    }

    public void AddListener<T0, T1>(string msg, System.Action<T0, T1> linstener)
    {
        if (!listenersMap.TryGetValue(msg, out List<object> linsteners))
        {
            linsteners = new List<object>();
            listenersMap.Add(msg, linsteners);
        }

        linsteners.Add(linstener);
    }

    public void SendMsg(string msg)
    {
        if (listenersMap.TryGetValue(msg, out List<object> linsteners))
        {
            foreach (var listener in linsteners)
            {
                if (listener is Action)
                    (listener as System.Action)();
                else if (listener is Delegate)
                {
                    (listener as Delegate).DynamicInvoke();
                }
            }
        }
    }

    public void SendMsg<T>(string msg, T arg)
    {
        if (listenersMap.TryGetValue(msg, out List<object> linsteners))
        {
            foreach (var listener in linsteners)
            {
                if (listener is System.Action<T> action)
                {
                    action(arg);
                }
                else if (listener is Delegate)
                {
                    (listener as Delegate).DynamicInvoke(arg);
                }
            }
        }
    }

    public void SendMsg<T0, T1>(string msg, T0 arg0, T1 arg1)
    {
        if (listenersMap.TryGetValue(msg, out List<object> linsteners))
        {
            foreach (var listener in linsteners)
            {
                if (listener is System.Action<T0, T1> action)
                {
                    (listener as System.Action<T0, T1>)(arg0, arg1);
                }
                else if (listener is Delegate)
                {
                    (listener as Delegate).DynamicInvoke(arg0,arg1);
                }
            }
        }
    }

    public void SendMsg(string msg, params object[] args)
    {
        if (listenersMap.TryGetValue(msg, out List<object> linsteners))
        {
            foreach (var listener in linsteners)
            {
                if (listener is Delegate)
                {
                    (listener as Delegate).DynamicInvoke(args);
                }
                else
                {
                    Debug.LogError("请使用带泛型的接口");
                }
            }
        }
    }

    public void RemoveListener(string msg, System.Action linstener)
    {
        if (listenersMap.TryGetValue(msg, out List<object> linsteners))
        {
            linsteners.Remove(linstener);
        }
    }

    public void RemoveListener<T>(string msg, System.Action<T> linstener)
    {
        if (listenersMap.TryGetValue(msg, out List<object> linsteners))
        {
            linsteners.Remove(linstener);
        }
    }

    public void RemoveListener<T0, T1>(string msg, System.Action<T0, T1> linstener)
    {
        if (listenersMap.TryGetValue(msg, out List<object> linsteners))
        {
            linsteners.Remove(linstener);
        }
    }

    public void RemmoveAllListener(string msg)
    {
        if (listenersMap.TryGetValue(msg, out List<object> linsteners))
        {
            linsteners.Clear();
        }
    }

    public void ClearAllListener()
    {
        foreach (var item in listenersMap.Values)
        {
            item.Clear();
        }
    }
}