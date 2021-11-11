using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class testBundle : MonoBehaviour
{
    GameObject obj;
    IEnumerator Start()
    {
        AssetBundleManager.Instance.Load("image", "Cube", delegate (UnityEngine.Object rOriginalRes, string rABName, string rResName)
        {
            obj = rOriginalRes as GameObject;
            GameObject.Instantiate(obj);
            Debug.Log(222);
        });
        AssetBundleManager.Instance.Load("image", "Image (1)", delegate (UnityEngine.Object rOriginalRes, string rABName, string rResName)
          {
              GameObject go = rOriginalRes as GameObject;
              GameObject.Instantiate(go, GameObject.Find("Canvas").transform);
              Debug.Log(111);
          });
       
        yield return new WaitForSeconds(5);
        //Resources.UnloadUnusedAssets();
        yield return new WaitForSeconds(2);
        AssetBundleManager.Instance.Load("image", "Image", delegate (UnityEngine.Object rOriginalRes, string rABName, string rResName)
        {
            obj = rOriginalRes as GameObject;
            GameObject.Instantiate(obj);
            Debug.Log(222);
        });

        AssetBundleManager.Instance.Load("newimage", "Imagett", delegate (UnityEngine.Object rOriginalRes, string rABName, string rResName)
        {
            obj = rOriginalRes as GameObject;
            //GameObject.Instantiate(obj, GameObject.Find("Canvas").transform);
           
            Debug.Log(111);
        });
        yield return new WaitForSeconds(2);


        foreach (var db in AssetBundleManager.Instance.get_mAssetBundleInfoDic())
        {
            if (db.Value.mABName == "border")
            {
                Debug.Log("状态"+db.Value.mABState);
                Debug.Log("ab包" + db.Value.mAssetBundle);
            }
            Debug.Log(db.Value.mABName + "引用" + db.Value.mRefCount);
        }

        SceneManager.LoadScene("test");

        //yield return new WaitForSeconds(2);
        //AssetBundleManager.Instance.UnloadAssetbundle_false("newimage");
        //Resources.UnloadUnusedAssets();
        //yield return new WaitForSeconds(2);
        //GameObject.Instantiate(obj, GameObject.Find("Canvas").transform);


        //AssetBundleManager.Instance.Release("image", false, delegate (UnityEngine.Object rOriginalRes, string rABName, string rResName)
        //  {

        //  });

        //AssetBundleManager.Instance.Release("image", false, delegate (UnityEngine.Object rOriginalRes, string rABName, string rResName)
        //{

        //});

    }

    private void OnDestroy()
    {
        AssetBundleManager.Instance.OnSceneChanged();
    }






    // Update is called once per frame
    void Update()
    {
        
    }
}
