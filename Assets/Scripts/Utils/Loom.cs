using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loom : MonoSingleton<Loom>
{
    static List<System.Action> netMsgHandles = new List<System.Action>();

    private Loom()
    {
        isGlobal = true;
    }

    // Update is called once per frame
    void Update()
    {
        lock (netMsgHandles)
        {
            foreach (var handle in netMsgHandles)
            {
                handle();
            }

            netMsgHandles.Clear();
        }
    }

    public static void AddNetMsgHandle(System.Action handle)
    {
        lock (netMsgHandles)
        {
            netMsgHandles.Add(handle);
        }
    }
}