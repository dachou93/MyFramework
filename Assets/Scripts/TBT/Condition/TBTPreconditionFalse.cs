using UnityEngine;
using System.Collections;
using System;

public class TBTPreconditionFalse : TBTPreconditionLeaf
{
    public override bool IsTrue(TBTEnemy baseTBT)
    {
        return false;
    }
}
