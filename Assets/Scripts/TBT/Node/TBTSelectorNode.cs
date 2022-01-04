using UnityEngine;
using System.Collections;

public class TBTSelectorNode : TBTActionNode
{
    private int currentselectNodeindex=-1;
    private int lastSelectNodeindex=-1;
    protected override bool OnEnter(TBTEnemy baseTBT)
    {
        for (int i = 0; i < GetChildCount(); i++)
        {
            TBTActionNode node = GetChild<TBTActionNode>(i);
            if(node.Enter(baseTBT))
            {
                currentselectNodeindex = i;
                return true;
            }
        }
        return false;
    }
    protected override RunStates OnUpdate(TBTEnemy baseTBT)
    {
        RunStates nodeState=RunStates.Completed;
       if (currentselectNodeindex!=lastSelectNodeindex)
        {
            if(lastSelectNodeindex!=-1)
            {
                TBTActionNode node = GetChild<TBTActionNode>(lastSelectNodeindex);
                node.Exit(baseTBT);
            }
            lastSelectNodeindex = currentselectNodeindex;
        }
       if(lastSelectNodeindex<GetChildCount()&&lastSelectNodeindex>=0)
        {
            TBTActionNode node = GetChild<TBTActionNode>(lastSelectNodeindex);
             nodeState=node.Update(baseTBT);
            if(nodeState==RunStates.Completed)
            {
                lastSelectNodeindex = -1;
            }
        }
        return nodeState;
    }
    protected override void OnExit(TBTEnemy baseTBT)
    {
        if (lastSelectNodeindex != -1)
        {
            TBTActionNode node = GetChild<TBTActionNode>(lastSelectNodeindex);
            if (node != null)
            {
                node.Exit(baseTBT);
            }
            lastSelectNodeindex = -1;
        }
    }
}
