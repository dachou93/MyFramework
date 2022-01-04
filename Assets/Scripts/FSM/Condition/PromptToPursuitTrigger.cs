using UnityEngine;
using System.Collections;
using AI.FSM;

namespace AI.FSM
{
    public class PromptToPursuitTrigger : FSMTrigger
    {
        public override void Init()
        {
            this.triggerId = FSMTriggerID.PromptToPursuit;
        }

        protected override bool Evaluate(BaseFSM fsm)
        {
            if (!fsm.IsPromptSkill())
            {
                if (fsm.targetEnemy != null)
                {
                    return fsm.targetDis > fsm.chStatus.attackDistance;
                }
            }
            return false;
        }
    }
}
