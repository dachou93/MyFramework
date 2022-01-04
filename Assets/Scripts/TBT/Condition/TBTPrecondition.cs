using UnityEngine;
using System.Collections;

public abstract class TBTPrecondition : TBTBaseNode
{
    public abstract bool IsTrue(TBTEnemy baseTBT);  
}
