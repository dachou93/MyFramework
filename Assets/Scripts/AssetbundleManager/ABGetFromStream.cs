using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ABGetFromStream : ABGetBase
{
    public ABGetFromStream(ABDecodeBase dBase) : base(dBase)
    {

    }

    public override IEnumerator Get_AssetBundel(string name, string path, Func<AssetBundle, IEnumerator> a)
    {
        using (var fileStream = new FileStream(path+name, FileMode.Open, FileAccess.Read, FileShare.None, 1024 * 4, false))
        {
            AssetBundleCreateRequest myLoadedAssetBundle = AssetBundle.LoadFromStreamAsync(fileStream);
            yield return myLoadedAssetBundle;
            if (!myLoadedAssetBundle.isDone)
            {
                Debug.LogError("Filed to get AssetBundles at " + name);
                //StartCoroutine(load_fromM(request.downloadHandler.data));
            }
            else
            {
                AssetBundle ab = myLoadedAssetBundle.assetBundle;
                if (a != null)
                {
                    AssetBundleManager.Instance.StartCoroutine(a(ab));
                }
            }
        }
    }

    public override IEnumerator Get_AssetBundel(string name, string path, Action<AssetBundle> a)
    {
        using (var fileStream = new FileStream(path + name, FileMode.Open, FileAccess.Read, FileShare.None, 1024 * 4, false))
        {
            AssetBundleCreateRequest myLoadedAssetBundle = AssetBundle.LoadFromStreamAsync(fileStream);
            yield return myLoadedAssetBundle;
            if (!myLoadedAssetBundle.isDone)
            {
                Debug.LogError("Filed to get AssetBundles at " + name);
                //StartCoroutine(load_fromM(request.downloadHandler.data));
            }
            else
            {
                AssetBundle ab = myLoadedAssetBundle.assetBundle;
                if (a != null)
                {
                    a(ab);
                }
            }
        }
    }
}
