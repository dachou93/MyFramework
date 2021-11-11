using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crater : MonoBehaviour
{
    void OnTriggerEnter(Collider coll)
    {
        //警报开启
        MessageMgr.Instance.SendMsg<GameObject>("EnterCrater", coll.gameObject);
    }
}
