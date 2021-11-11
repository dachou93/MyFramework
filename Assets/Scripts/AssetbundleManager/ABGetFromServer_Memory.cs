using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ABGetFromServer_Memory : ABGetBase
{
    public ABGetFromServer_Memory(ABDecodeBase dBase) : base(dBase)
    {

    }
    public override IEnumerator Get_AssetBundel(string name, string path, Func<AssetBundle, IEnumerator> a)
    {
        UnityWebRequest request = UnityWebRequest.Get(path + name);
        yield return request.SendWebRequest();
        if (!request.isDone)
        {
            Debug.LogError("Filed to get AssetBundles at "+name);
            //StartCoroutine(load_fromM(request.downloadHandler.data));
        }
        else
        {
            byte[] b = decodeBase.Decode_byte(request.downloadHandler.data);
            AssetBundleCreateRequest req = AssetBundle.LoadFromMemoryAsync(b);
            yield return req;
            AssetBundle ab = req.assetBundle;
            if (a != null)
            {
                AssetBundleManager.Instance.StartCoroutine(a(ab));
            }
        }
    }

    public override IEnumerator Get_AssetBundel(string name, string path, Action<AssetBundle> a)
    {
        UnityWebRequest request = UnityWebRequest.Get(path + name);
        yield return request.SendWebRequest();
        if (!request.isDone)
        {
            Debug.LogError("Filed to get AssetBundles");
            //StartCoroutine(load_fromM(request.downloadHandler.data));
        }
        else
        {
            byte[] b = decodeBase.Decode_byte(request.downloadHandler.data);
            AssetBundleCreateRequest req = AssetBundle.LoadFromMemoryAsync(b);
            yield return req;
            AssetBundle ab = req.assetBundle;
            if (a != null)
            {
                a(ab);
            }
        }
    }

}
