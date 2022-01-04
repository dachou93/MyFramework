using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TBTPreconditionTrue : TBTPreconditionLeaf
{
    public override bool IsTrue(TBTEnemy baseTBT)
    {
        return true;
    }
}
