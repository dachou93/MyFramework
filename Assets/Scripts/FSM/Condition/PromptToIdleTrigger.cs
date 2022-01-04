using UnityEngine;
using System.Collections;
using AI.FSM;

namespace AI.FSM
{
    public class PromptToIdleTrigger : FSMTrigger
    {
        public override void Init()
        {
            this.triggerId = FSMTriggerID.PromptToIdle;
        }

        protected override bool Evaluate(BaseFSM fsm)
        {
            if(!fsm.IsPromptSkill()&&fsm.OrderPoint==fsm.DefaultPoint&&fsm.targetEnemy==null)
            {
                return true;
            }
            return false;
        }
    }
}
