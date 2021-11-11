using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class ABGetBase 
{
    protected ABDecodeBase decodeBase;
    public ABGetBase(ABDecodeBase dbase)
    {
        decodeBase = dbase;
    }

    public abstract IEnumerator Get_AssetBundel(string name, string path, Func<AssetBundle,IEnumerator> a);

    public abstract IEnumerator Get_AssetBundel(string name, string path, Action<AssetBundle> a);

}
