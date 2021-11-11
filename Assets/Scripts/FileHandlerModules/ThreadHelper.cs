using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ThreadHelper
{
    private static ThreadHelper instance = null;

    public static ThreadHelper getInstance()
    {
        if (instance == null)
        {
            instance = new ThreadHelper();
        }

        return instance;
    }

    static int WorkerCounts = 4;

    private ThreadHelper()
    {
        CreateWorkers(WorkerCounts);
        msgDistribution = new MsgDistribution();
    }

    public void OnDestory()
    {
        for (int i = 0; i < workers.Length; i++)
        {
            workers[i].OnDestroy();
        }
    }

    MsgDistribution msgDistribution;

    private WorkerThread[] workers;

    public void CreateWorkers(int count)
    {
        workers = new WorkerThread[count];
        for (int i = 0; i < count; i++)
        {
            workers[i] = new WorkerThread();
        }
    }

    private WorkerThread GetWorker(int index)
    {
        index = Mathf.Abs(index);
        index = index % WorkerCounts;
        if (index >= workers.Length)
            return null;
        return workers[index];
    }

    public void QueueWorkItem(int index, Action task)
    {
        WorkerThread worker = GetWorker(index);
        if (task == null)
        {
            worker = GetWorker(task.GetHashCode() % workers.Length);
        }
        worker.QueueWorkItem(task);
    }

    public void QueueWorkItem(Action task)
    {
        WorkerThread worker = GetWorker(task.GetHashCode() % workers.Length);
        worker.QueueWorkItem(task);
    }

    public void QueueMainItem(Action task)
    {
        msgDistribution.Add_Task(task);
    }

    public void Run()
    {
        msgDistribution.Run();
    }
}
