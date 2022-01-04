    using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ECHILDREN_RELATIONSHIP
{
    AND, OR
}
public class TBTParallelNode :TBTActionNode
{
    private ECHILDREN_RELATIONSHIP EnterRelationship;
    private ECHILDREN_RELATIONSHIP UpdateStatusRelationship;
    public TBTParallelNode()
            : base()
        {
        EnterRelationship = ECHILDREN_RELATIONSHIP.AND;
        UpdateStatusRelationship = ECHILDREN_RELATIONSHIP.OR;
    }
    public TBTParallelNode SetEvaluationRelationship(ECHILDREN_RELATIONSHIP v)
    {
        EnterRelationship = v;
        return this;
    }
    public TBTParallelNode SetRunningStatusRelationship(ECHILDREN_RELATIONSHIP v)
    {
        UpdateStatusRelationship = v;
        return this;
    }
    protected override bool OnEnter(TBTEnemy baseTBT)
    {
        bool finalResult = false;
        for (int i = 0; i < GetChildCount(); i++)
        {
            TBTActionNode node = GetChild<TBTActionNode>(i);
            bool ret = node.Enter(baseTBT);
            if(EnterRelationship==ECHILDREN_RELATIONSHIP.AND && ret==false)
            {
                finalResult = false;
                break;
            }
            if (ret == true)
            {
                finalResult = true;
            }
        }
        return finalResult;
    }
    protected override RunStates OnUpdate(TBTEnemy baseTBT)
    {
        RunStates stat=RunStates.Completed;
        bool ret = false;
        for (int i = 0; i < GetChildCount(); i++)
        {
            TBTActionNode node = GetChild<TBTActionNode>(i);
            RunStates nodestate=node.Update(baseTBT);
            if(nodestate==RunStates.Running)
            {
                ret = true;
                stat = RunStates.Running;
            }
        }
        if (ret)
        {
            return RunStates.Running;
        }
        else
        {
            return stat;
        }
    }
    protected override void OnExit(TBTEnemy baseTBT)
    {
        for (int i = 0; i < GetChildCount(); i++)
        {
            TBTActionNode node = GetChild<TBTActionNode>(i);
            node.Exit(baseTBT);
        }
    }
}
