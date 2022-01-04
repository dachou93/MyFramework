using UnityEngine;
using System.Collections;
using AI.FSM;
using System;


namespace AI.FSM
{
    public class GetTargetTrigger : FSMTrigger
    {
        public override void Init()
        {
            this.triggerId = FSMTriggerID.GetTarget;
        }

        protected override bool Evaluate(BaseFSM fsm)
        {
            if(fsm.targetEnemy != null)
            {
                if (fsm.UsedSkill.Count == 0)
                    return fsm.targetDis > fsm.chStatus.attackDistance;
                else
                    return fsm.targetDis > fsm.UsedSkill.Peek().attackDistance;
            }

            return false;
        }
    }
}
