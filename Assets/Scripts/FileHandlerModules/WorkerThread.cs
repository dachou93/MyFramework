using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WorkerThread 
{
    private object obj;
    private ConcurrentQueue<Action> taskQueueInSub;
    int wakenUp;
    Thread thread;
    public WorkerThread()
    {
        obj = new object();
        taskQueueInSub = new ConcurrentQueue<Action>();
        thread = new Thread(RunInSub);
        thread.Start();
    }

    void RunInSub(object o)
    {
        while (true)
        {
            try
            {
                lock (obj)
                {
                    //阻塞状态排除问题
                    Interlocked.Exchange(ref wakenUp, 0);
                    if (taskQueueInSub.IsEmpty)
                    {
                        Monitor.Wait(obj);
                    }
                }
                //处理work任务
                while (true)
                {
                    Action task;
                    taskQueueInSub.TryDequeue(out task);
                    if (task == null)
                    {
                        break;
                    }
                    //存在work任务开始执行
                    task.Invoke();
                }
            }
            catch (Exception e)
            {

                if (e is ThreadAbortException)
                {
                    Debug.Log("线程已关闭");
                }
                else
                {
                    Debug.LogError(e);
                }
            }
        }
    }

    private int CompareAndSet(int value, int comparand)
    {
        Interlocked.CompareExchange(ref wakenUp, value, comparand);
        return wakenUp;
    }

     void NotifyWorker()
    {
        if (CompareAndSet(1,0)==1)
        {
            lock (obj)
            {
                Monitor.Pulse(obj);
            }
        }
    }

    public void QueueWorkItem(Action task)
    {
        taskQueueInSub.Enqueue(task);
        NotifyWorker();
    }

    public void OnDestroy()
    {
        thread.Abort();
    }
}
