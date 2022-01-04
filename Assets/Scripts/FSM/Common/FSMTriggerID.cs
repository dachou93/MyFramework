using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI.FSM
{

    public enum FSMTriggerID
    {
        NoHealth,            //生命为0	
        GetOrder,          //获取指令
        GetTarget,          //设置目标
        ReachOrder,          //到达指令地点
        ReachTarget,         //到达目标
        NotReachTarget,       //没到目标
        EnemyDie,
        UseSkill,              //可以使用技能
        SkillToPursuit,
        SkillToAttack,
        IsPrompt,
        PromptToPursuit,
        PromptToIdle,
        PromptToAttack,
        IsControl,
        UnControl
    }
}
