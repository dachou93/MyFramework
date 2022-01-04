using UnityEngine;
using System.Collections;
using AI.FSM;

namespace AI.FSM
{
    public class EnemyDieTrigger : FSMTrigger
    {
        public override void Init()
        {
            this.triggerId = FSMTriggerID.EnemyDie;
        }

        protected override bool Evaluate(BaseFSM fsm)
        {
            if (fsm.targetEnemy != null)
            {
                if (fsm.targetEnemy.GetComponent<CharacterStatus>().HP <= 0)
                {
                    fsm.targetEnemy = null;
                    return true;
                }
            }
            return false;
        }
    }
}
