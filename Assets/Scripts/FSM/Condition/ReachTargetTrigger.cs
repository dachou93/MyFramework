using UnityEngine;
using System.Collections;
using AI.FSM;

namespace AI.FSM
{
    public class ReachTargetTrigger : FSMTrigger
    {
        public override void Init()
        {
            this.triggerId = FSMTriggerID.ReachTarget;
        }

        protected override bool Evaluate(BaseFSM fsm)
        {
            if (fsm.targetEnemy != null)
            {
                return fsm.targetDis < fsm.chStatus.attackDistance;
            }
            return false;
        }
    }
}
