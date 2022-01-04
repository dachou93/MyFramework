using UnityEngine;
using System.Collections;
using AI.FSM;

namespace AI.FSM
{
    public class ReachOrderTrigger : FSMTrigger
   {
    public override void Init()
    {
        this.triggerId = FSMTriggerID.ReachOrder;
    }

    protected override bool Evaluate(BaseFSM fsm)
    {
            Vector3 heroPoint = fsm.transform.position;
            return Vector3.Distance(fsm.OrderPoint, new Vector3(heroPoint.x, fsm.OrderPoint.y, heroPoint.z)) <= 0.5f;
    }
  }
}
