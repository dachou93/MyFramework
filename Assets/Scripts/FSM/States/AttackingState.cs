using System;
using System.Collections.Generic;
using UnityEngine;

    namespace AI.FSM
    {
    public class AttackingState : FSMState
    {
        //两次攻击计时
        private float attackTime;

        public override void Init()
        {
            stateId = FSMStateID.Attacking;
        }

        public override void EnterState(BaseFSM fsm)
        {
            //if (attackTime == 0)
            //attackTime = fsm.chStatus.attackSpeed;
            //fsm.transform.rotation =Quaternion.LookRotation(fsm.targetDir);
            //fsm.chSystem.AttackUseSkill(1, false);
            attackTime = 0;

        }

        public override void ExitState(BaseFSM fsm)
        {
           fsm.PlayAnimation(fsm.animParams.Attack, false);
        }

        public override void Action(BaseFSM fsm)
        {
            if (!fsm.IsAttacking())
            {
                if (attackTime <= 0)
                {
                    attackTime = fsm.chStatus.attackSpeed;
                    fsm.transform.rotation = Quaternion.LookRotation(fsm.targetDir);
                    fsm.singleTarget = fsm.targetEnemy;
                    fsm.chSystem.AttackUseSkill(1, false);
                
            }
                attackTime -= Time.deltaTime;
            }
        }
    }
}
