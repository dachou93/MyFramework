using UnityEngine;
using System.Collections;
using AI.FSM;

namespace AI.FSM
{
    public class UseSkillTrigger : FSMTrigger
    {
        public override void Init()
        {
            this.triggerId = FSMTriggerID.UseSkill;
        }

        protected override bool Evaluate(BaseFSM fsm)
        {
            if (fsm.targetEnemy != null)
            {
                if (fsm.UsedSkill.Count != 0)
                {
                    return fsm.targetDis <= fsm.UsedSkill.Peek().attackDistance;
                }
            }
            return false;
        }
    }
}
