using UnityEngine;
using System.Collections;

public class TBTActionNode :TBTBaseNode
{
    //protected RunStates status = RunStates.Completed;
    private string name;
    public string Name { get; set; }
    protected TBTPrecondition precondition;
    public bool Enter(TBTEnemy baseTBT)
    {
        return (precondition == null || precondition.IsTrue(baseTBT)) && OnEnter(baseTBT);
    }
    protected virtual bool OnEnter(TBTEnemy baseTBT)    
    {
        return true;
    }
    public RunStates Update(TBTEnemy baseTBT)
    {
        return OnUpdate(baseTBT);
    }
    protected virtual RunStates OnUpdate(TBTEnemy baseTBT)
    {
        return RunStates.Completed;
    }
    public void Exit(TBTEnemy baseTBT)
    {
        OnExit(baseTBT);
    }
    protected virtual void OnExit(TBTEnemy baseTBT)
    {
    }
    public TBTActionNode SetPrecondition(TBTPrecondition precondition)
    {
        this.precondition = precondition;
        return this;
    }


}
