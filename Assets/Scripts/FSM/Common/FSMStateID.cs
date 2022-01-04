using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI.FSM
{
    public enum FSMStateID
    {
        Pursuit,        //追逐	
        Dead,           //死亡	
        Attacking,  //攻击	
        Idle,            //待机	
        Default,        //默认	
        Arrival,        //到达	
        None,           //无
        Move,            //移动
        Skill,            //使用技能
        Prompt,
        Control

    }
}
