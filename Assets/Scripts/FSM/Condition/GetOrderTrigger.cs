using UnityEngine;
using System.Collections;
using AI.FSM;
using System;

namespace AI.FSM
{
    public class GetOrderTrigger : FSMTrigger
    {
        public override void Init()
        {
            this.triggerId = FSMTriggerID.GetOrder;
        }

        protected override bool Evaluate(BaseFSM fsm)
        {
            if(!fsm.IsUsingSkill())
                return fsm.OrderPoint != new Vector3(100,100,100);
            return false;
        }
    }
}
