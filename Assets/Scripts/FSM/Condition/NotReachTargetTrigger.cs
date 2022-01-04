using UnityEngine;
using System.Collections;
using AI.FSM;

namespace AI.FSM
{
    public class NotReachTargetTrigger : FSMTrigger
    {
        public override void Init()
        {
            this.triggerId = FSMTriggerID.NotReachTarget;
        }

        protected override bool Evaluate(BaseFSM fsm)
        {
            if (fsm.targetEnemy != null)
            {
                return fsm.targetDis > fsm.chStatus.attackDistance;
            }
            return false;
        }
    }
}
