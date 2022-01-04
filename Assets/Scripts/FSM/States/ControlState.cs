using UnityEngine;
using System.Collections;
using System;

namespace AI.FSM
{
    public class ControlState : FSMState
    {
        public override void Action(BaseFSM fsm)
        {
         
        }

        public override void EnterState(BaseFSM fsm)
        {
           
        }

        public override void ExitState(BaseFSM fsm)
        {
            fsm.OrderPoint = fsm.DefaultPoint;
            fsm.targetEnemy = null;
        }

        public override void Init()
        {
            stateId = FSMStateID.Control;
        }
    }
}
