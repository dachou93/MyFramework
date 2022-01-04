using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FSM
{
	public class PursuitState : FSMState 
	{
        private Transform currentTarget;
        private Vector3 dir;
        public override void Init()
        {
            stateId = FSMStateID.Pursuit;
        }

        public override void EnterState(BaseFSM fsm)
        {
            fsm.PlayAnimation(fsm.animParams.Run, true);
            SetDir(fsm);
        }

        public override void ExitState(BaseFSM fsm)
        {
            fsm.PlayAnimation(fsm.animParams.Run, false);
            currentTarget = null;
        }

        public override void Action(BaseFSM fsm)
        {
            SetDir(fsm);
            fsm.characterMotor.Movement(dir);
        }
        private void SetDir(BaseFSM fsm)
        {
            if (fsm.targetEnemy != null)
            {
                    dir = fsm.targetEnemy.position - fsm.transform.position;
                    dir = new Vector3(dir.x, 0, dir.z);
                    currentTarget = fsm.targetEnemy;
            }
        }
    }
}
