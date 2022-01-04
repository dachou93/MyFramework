using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FSM
{
	public class DeadState : FSMState 
	{
        public override void Init()
        {
            stateId = FSMStateID.Dead;
        }

        public override void EnterState(BaseFSM fsm)
        {
            fsm.PlayAnimation(fsm.animParams.Dead, true);
        }

        public override void ExitState(BaseFSM fsm)
        {
        }

        public override void Action(BaseFSM fsm)
        {
        }
    }
}
