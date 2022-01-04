using UnityEngine;
using System.Collections;
using AI.FSM;

namespace AI.FSM
{
    public class IsPromptTrigger : FSMTrigger
    {
        public override void Init()
        {
            this.triggerId = FSMTriggerID.IsPrompt;
        }

        protected override bool Evaluate(BaseFSM fsm)
        {
            return fsm.IsPromptSkill();
        }
    }
}
