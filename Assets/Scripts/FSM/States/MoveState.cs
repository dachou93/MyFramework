using UnityEngine;
using System.Collections;
using AI.FSM;

namespace AI.FSM
{
    public class MoveState : FSMState
    {
        private Vector3 dir;
        private Vector3 currentOderPoint = new Vector3(200, 200, 200);
        public override void Init()
        {
            stateId = FSMStateID.Move;
        }

        public override void EnterState(BaseFSM fsm)
        {
            fsm.PlayAnimation(fsm.animParams.Run, true);
            SetDir(fsm);
        }

        public override void ExitState(BaseFSM fsm)
        {
            fsm.PlayAnimation(fsm.animParams.Run, false);
            fsm.OrderPoint = fsm.DefaultPoint;
            currentOderPoint = new Vector3(200, 200, 200);
        }

        public override void Action(BaseFSM fsm)
        {
            SetDir(fsm);
            fsm.characterMotor.Movement(dir);
        }
        private void SetDir(BaseFSM fsm)
        {
            if (currentOderPoint != fsm.OrderPoint)
            {
                dir = fsm.OrderPoint - fsm.transform.position;
                dir = new Vector3(dir.x, 0, dir.z);
                currentOderPoint = fsm.OrderPoint;
            }
        }
    }
}
