using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TBTActionLeaf : TBTActionNode
{
    protected RunStates states = RunStates.READY;
    protected bool needExit;
    protected sealed override RunStates OnUpdate(TBTEnemy baseTBT)
    {
        RunStates runningState = RunStates.Completed;
        if (states == RunStates.READY)
        {
            OnActionEnter(baseTBT);
            needExit = true;
            states = RunStates.Running;
        }
        if (states == RunStates.Running)
        {
            runningState = OnActionUpdate(baseTBT);
            if (runningState == RunStates.Completed)
            {
                states = RunStates.Completed;
            }
        }
        if (states == RunStates.Completed)
        {
            if (needExit)
            {
                OnActionExit(baseTBT);
            }
            states = RunStates.READY;
            needExit = false;

        }
        return runningState;
    }
    protected sealed override void OnExit(TBTEnemy baseTBT)
    {
        if (needExit)
        {
            OnActionExit(baseTBT);
        }
        states = RunStates.READY;
        needExit = false;

    }
    protected virtual void OnActionEnter(TBTEnemy baseTBT)
    { }
    protected virtual RunStates OnActionUpdate(TBTEnemy baseTBT)
    { return RunStates.Completed; }
    protected virtual void OnActionExit(TBTEnemy baseTBT)
    { }
}