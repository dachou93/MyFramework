using UnityEngine;
using System.Collections;
using AI.FSM;

namespace AI.FSM
{
    public class SkillToPursuitTrigger : FSMTrigger
    {
        public override void Init()
        {
            this.triggerId = FSMTriggerID.SkillToPursuit;
        }

        protected override bool Evaluate(BaseFSM fsm)
        {
            if (!fsm.IsUsingSkill())
            {
                if (fsm.targetEnemy != null)
                {
                    if (fsm.UsedSkill.Count != 0)
                    {
                        return fsm.targetDis > fsm.UsedSkill.Peek().attackDistance;
                    }
                    else
                        return fsm.targetDis > fsm.chStatus.attackDistance;
                }
            }
            return false;
        }
    }
}
