using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tmy 
{
    IEnumerator bb()
    {
        Debug.Log(2);
        yield return new WaitForSeconds(3);
        Debug.Log(4);
    }

    public void test()
    {
        AssetBundleManager.Instance.StartCoroutine(bb());
    }
}
