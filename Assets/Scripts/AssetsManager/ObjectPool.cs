using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    static GameObject poolRoot;

    Dictionary<string, LinkedList<object>> objectPool = new Dictionary<string, LinkedList<object>>();
    Dictionary<object, string> objectIDDic = new Dictionary<object, string>();

    Dictionary<object, Lifecycle> objLifecycleDic = new Dictionary<object, Lifecycle>();

    // 一直需要存在的obj
    HashSet<object> allTheTimeObjs = new HashSet<object>();

    // 当前场景需要存在的obj
    HashSet<object> currentSceneObjs = new HashSet<object>();

    // 不需要存在，可随时释放的obj
    HashSet<object> zeroObjs = new HashSet<object>();

    HashSet<object>[] lifecycleArr = new HashSet<object>[3];

    private ObjectPool()
    {
        poolRoot = new GameObject("ObjectPool");
        poolRoot.transform.position = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        poolRoot.SetActive(false);

        lifecycleArr[0] = zeroObjs;
        lifecycleArr[1] = currentSceneObjs;
        lifecycleArr[2] = allTheTimeObjs;

        Object.DontDestroyOnLoad(poolRoot);
    }

    // 弹出对象
    public T Pop<T>(string id) where T : class
    {
        if (objectPool.TryGetValue(id, out LinkedList<object> queue))
        {
            if (queue.Count > 0)
            {
                object obj = queue.First.Value;
                queue.RemoveFirst();

                if (queue.Count == 0)
                {
                    objectPool.Remove(id);
                }

                objectIDDic.Remove(obj);

                lifecycleArr[(int)objLifecycleDic[obj]].Remove(obj);
                objLifecycleDic.Remove(obj);

                return obj as T;
            }
        }

        return null;
    }

    // 查看对象
    public T Peek<T>(string id, Lifecycle lifecycle) where T : class
    {
        if (objectPool.TryGetValue(id, out LinkedList<object> queue))
        {
            if (queue.Count > 0)
            {
                object obj = queue.First.Value;

                // 修改生命周期
                if (objLifecycleDic[obj] != lifecycle)
                {
                    lifecycleArr[(int)objLifecycleDic[obj]].Remove(obj);
                    lifecycleArr[(int)lifecycle].Add(obj);

                    objLifecycleDic[obj] = lifecycle;
                }

                return obj as T;
            }
        }

        return null;
    }

    // 存入对象
    public void Push<T>(string id, T obj, Lifecycle lifecycle, bool maybeGameObject)
    {
        if (!objectPool.TryGetValue(id, out LinkedList<object> queue))
        {
            queue = new LinkedList<object>();
            objectPool.Add(id, queue);
        }

        queue.AddLast(obj);

        objectIDDic.Add(obj, id);

        objLifecycleDic.Add(obj, lifecycle);
        lifecycleArr[(int)lifecycle].Add(obj);

        if (maybeGameObject && obj as GameObject)
        {
            (obj as GameObject).transform.SetParent(poolRoot.transform);
        }
    }

    public string Dump()
    {
        int count = 0;
        foreach (var pair in objectPool)
        {
            foreach (var objPair in pair.Value)
            {
                count++;
            }
        }

        return $"对象池中: 一直需要存在的obj个数:{allTheTimeObjs.Count} 当前场景需要存在的obj个数:{currentSceneObjs.Count} 可以释放的obj个数:{zeroObjs.Count}";
    }

    // 用于切换场景时，把当前场景生命周期的obj移到zero生命周期中去
    public void Recycle()
    {
        foreach (var obj in lifecycleArr[1])
        {
            zeroObjs.Add(obj);

            // 修改生命周期
            objLifecycleDic[obj] = Lifecycle.Zero;
        }

        currentSceneObjs.Clear();

        List<object> removeList = new List<object>();

        // 暂时策略 如果生命周期为zero，则销毁对象
        foreach (var obj in zeroObjs)
        {
            GameObject gameObject = obj as GameObject;

            if (gameObject && gameObject.name.Contains("Clone"))
            {
                Object.Destroy(obj as GameObject);

                objLifecycleDic.Remove(obj);

                objectPool[objectIDDic[obj]].Remove(obj);
            }
        }

        foreach (var obj in removeList)
        {
            zeroObjs.Remove(obj);
        }
    }
}

public enum Lifecycle
{
    Zero,
    Current_Scene,
    All_The_Time
}