using UnityEngine;
using System.Collections;
using AI.FSM;

namespace AI.FSM
{
    public class PromptState : FSMState
    {
        public override void Init()
        {
            stateId = FSMStateID.Prompt;
        }

        public override void EnterState(BaseFSM fsm)
        {
        }


        public override void ExitState(BaseFSM fsm)
        {

        }

        public override void Action(BaseFSM fsm)
        {

        }

    }
}

