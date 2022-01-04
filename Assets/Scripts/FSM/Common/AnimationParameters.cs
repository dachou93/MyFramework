using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FSM
{
    /// <summary>为动画状态机提供的参数</summary>
    [Serializable]
	public class AnimationParameters
	{
        public string Idle = "Idle";
        public string Dead = "dead";
        public string Run = "run";
        public string Attack = "attack";
        public string Walk = "walk";
        public string Control = "control";
        #region MT
        public string Attack2 = "attack2";
        public string MTCharge = "charge";
        public string MTAddCharge = "addcharge";
        public string MTAddDEF = "addDEF";
        #endregion
        #region CK
        public string CKAddDamage = "addDamage";
        public string CKHighDamage = "highDamage";
        public string CKAddReduceDEF = "addReduceDEF";
        public string CKReduceDEF = "reduceDEF";
        public string CKAddFlash = "addFlash";
        public string CKFlash = "Flash";
        #endregion
        #region FS
        public string FSAddIce = "addIce";
        public string FsIce = "ice";
        public string FSJump = "jump";
        #endregion
        #region ZS
        public string ZSFTYD = "spelldark";
        public string ZSFT = "share";
        public string ZSFireYD = "spellfire";
        public string ZSFire = "fire";
        #endregion
    }
}
