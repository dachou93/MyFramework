using UnityEngine;
using System.Collections;

namespace AI.FSM
{
    public class IsControlTrigger : FSMTrigger
    {
        public override void Init()
        {
            this.triggerId = FSMTriggerID.IsControl;
        }

        protected override bool Evaluate(BaseFSM fsm)
        {
            if (fsm.IsControl())
            {
                return true;
            }
            return false;
        }
    }
}
