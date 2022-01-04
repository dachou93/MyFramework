using UnityEngine;
using System.Collections;

public class TBTSequenceNode : TBTActionNode
{
    private int currentSelectindex=-1;
    protected override bool OnEnter(TBTEnemy baseTBT)
    {
        int checkedNodeIndex = -1;
        if (currentSelectindex>=0&&currentSelectindex<GetChildCount())
        {
            checkedNodeIndex = currentSelectindex;
        }
        else
        {
            checkedNodeIndex = 0;
        }
        if(checkedNodeIndex>= 0 && currentSelectindex < GetChildCount())
        {
            TBTActionNode node = GetChild<TBTActionNode>(checkedNodeIndex);
            if(node.Enter(baseTBT))
            {
                currentSelectindex = checkedNodeIndex;
                return true;
            }
        }
        return false;
    }
    protected override RunStates OnUpdate(TBTEnemy baseTBT)
    {
        TBTActionNode node = GetChild<TBTActionNode>(currentSelectindex);
        RunStates runstates = node.Update(baseTBT);
        if(runstates==RunStates.Completed)
        {
            currentSelectindex++;
            if (currentSelectindex >= 0 && currentSelectindex < GetChildCount())
            {
                runstates = RunStates.Running;
            }
            else
            {
                currentSelectindex = -1;
            }
        }
        return runstates;
    }
    protected override void OnExit(TBTEnemy baseTBT)
    {
        TBTActionNode node = GetChild<TBTActionNode>(currentSelectindex);
        if(node!=null)
        {
            node.Exit(baseTBT);
        }
        currentSelectindex = -1;
    }
}
