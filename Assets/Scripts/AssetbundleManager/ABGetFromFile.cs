using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABGetFromFile : ABGetBase
{

    public ABGetFromFile(ABDecodeBase dBase) : base(dBase)
    {

    }
    public override IEnumerator Get_AssetBundel(string name, string path, Func<AssetBundle, IEnumerator> a)
    {
        int offset = decodeBase.Decode_Offset(name);
        AssetBundleCreateRequest rRequest = AssetBundle.LoadFromFileAsync(path + name,0,(ulong)offset);
        yield return rRequest;
        if (!rRequest.isDone)
        {
            Debug.LogError("Fail load ab file at " + name);
            yield break;
        }
        else
        {
           AssetBundle ab = rRequest.assetBundle;
            if (a != null)
            {
               AssetBundleManager.Instance.StartCoroutine(a(ab));
            }
        }
     
        
    }

    public override IEnumerator Get_AssetBundel(string name, string path,Action<AssetBundle> a)
    {
        int offset = decodeBase.Decode_Offset(name);
        AssetBundleCreateRequest rRequest = AssetBundle.LoadFromFileAsync(path + name, 0, (ulong)offset);
        yield return rRequest;
        if (!rRequest.isDone)
        {
            Debug.LogError("Fail load ab file at " + name);
            yield break;
        }
        else
        {
            AssetBundle ab = rRequest.assetBundle;
            if (a != null)
            {
                a(ab);
            }
        }


    }
}
