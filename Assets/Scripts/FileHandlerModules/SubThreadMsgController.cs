using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubThreadMsgController : Controller
{

    public SubThreadMsgController()
    {
        GlobalScheduler.Instance.AddUpdateEvent(() =>
        {
            LoopGetMsg();
        });

        GlobalScheduler.Instance.AddDestoryEvent(delegate {
            OnDestroy();
        });
    }

    public void LoopGetMsg()
    {
        ThreadHelper.getInstance().Run();
    }

    private void OnDestroy()
    {
        ThreadHelper.getInstance().OnDestory();
    }

}
