using UnityEngine;
using System.Collections;
using AI.FSM;

namespace AI.FSM
{
    public class PromptToAttackTrigger : FSMTrigger
    {
        public override void Init()
        {
            this.triggerId = FSMTriggerID.PromptToAttack;
        }

        protected override bool Evaluate(BaseFSM fsm)
        {
            if (!fsm.IsPromptSkill())
            {
                if (fsm.targetEnemy != null)
                {
                    return fsm.targetDis <= fsm.chStatus.attackDistance;
                }
            }
            return false;
        }
    }
}
