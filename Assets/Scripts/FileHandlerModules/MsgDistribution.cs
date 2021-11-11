using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;

//消息分发
public class MsgDistribution
{
    public MsgDistribution()
    {
        tasks = new ConcurrentQueue<Action>();
    }

    private ConcurrentQueue<Action> tasks;

    //主线程没帧执行
    public void Run()
    {
        while (true)
        {
            Action task;
            tasks.TryDequeue(out task);
            if (task == null)
            {
                break;
            }
            task.Invoke();
        }

    }

    public void Add_Task(Action task)
    {
        tasks.Enqueue(task);
    }

   
}