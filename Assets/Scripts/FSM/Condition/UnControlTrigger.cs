using UnityEngine;
using System.Collections;

namespace AI.FSM
{
    public class UnControlTrigger : FSMTrigger
    {
        public override void Init()
        {
            this.triggerId = FSMTriggerID.UnControl;
        }

        protected override bool Evaluate(BaseFSM fsm)
        {
            if (!fsm.IsControl())
            {
                return true;
            }
            return false;
        }
    }
}
