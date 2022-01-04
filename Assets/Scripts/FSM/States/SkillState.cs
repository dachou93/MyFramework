using UnityEngine;
using System.Collections;
using AI.FSM;

namespace AI.FSM
{
    public class SkillState : FSMState
    {
        //两次攻击计时
        private float attackTime;
        private SkillData currentSkill;

        public override void Init()
        {
            stateId = FSMStateID.Skill;
        }

        public override void EnterState(BaseFSM fsm)
        {

            attackTime = 0;
            //currentSkill = fsm.UsedSkill.Pop();
            //fsm.transform.rotation = Quaternion.LookRotation(fsm.targetDir);
            //fsm.chSystem.AttackUseSkill(currentSkill.skillID, false);
        }

        public override void ExitState(BaseFSM fsm)
        {
            fsm.PlayAnimation(currentSkill.animationName, false);
        }

        public override void Action(BaseFSM fsm)
        {
            if (!fsm.IsUsingSkill())
            {
                if (attackTime <= 0)
                {
                    if (fsm.UsedSkill.Count != 0)
                    {
                        attackTime = fsm.chStatus.attackSpeed;
                        currentSkill = fsm.UsedSkill.Pop();
                        fsm.transform.rotation = Quaternion.LookRotation(fsm.targetDir);
                        fsm.singleTarget = fsm.targetEnemy;
                        fsm.chSystem.AttackUseSkill(currentSkill.skillID, false);
                    }
                }
                attackTime -= Time.deltaTime;
            }
        }
    }
}
