using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ABGetFromServer : ABGetBase
{

    public ABGetFromServer(ABDecodeBase dBase):base(dBase)
    {

    }

    public override IEnumerator Get_AssetBundel(string name, string path, Func<AssetBundle, IEnumerator> a)
    {
        UnityWebRequest request =UnityWebRequestAssetBundle.GetAssetBundle(path+name);

        yield return request.SendWebRequest();
        if (!request.isDone)
        {
            Debug.LogError("Fail load Mainmanifest file at " + name);
            yield break;
        }
        else
        {
            AssetBundle ab = (request.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
            if (a != null)
            {
                AssetBundleManager.Instance.StartCoroutine(a(ab));
            }
        }
    }

    public override IEnumerator Get_AssetBundel(string name, string path, Action<AssetBundle> a)
    {
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(path + name);

        yield return request.SendWebRequest();
        if (!request.isDone)
        {
            Debug.LogError("Fail load Mainmanifest file at " + name);
            yield break;
        }
        else
        {
            AssetBundle ab = (request.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
            if (a != null)
            {
                a(ab);
            }
        }
    }
}
