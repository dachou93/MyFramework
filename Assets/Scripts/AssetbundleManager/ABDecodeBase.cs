using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AssetBundleManager;

public  class ABDecodeBase
{
    public virtual int Decode_Offset(string name)
    {
        return 0;
    }

    public virtual byte[] Decode_byte(byte[] b)
    {
        Debug.Log("bb");
        return b;
    }


}

public class ABDecodeOffSet: ABDecodeBase
{
    public override int Decode_Offset(string name)
    {
        Dictionary<string, AssetReqBaseInfo> dic = AssetBundleManager.Instance.get_mAssetBundleInfoDic();

        if (!dic.ContainsKey(name))
            return 0;
        string hashcode = dic[name].ABHash;
        int offset = Mathf.Abs(hashcode.GetHashCode() % 10);
        if (offset == 0)
            offset++;
        return offset;
    }
}