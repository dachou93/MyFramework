using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

namespace AI.FSM
{
    /// <summary>
    /// 有限状态机
    /// </summary>
    public class BaseFSM : MonoBehaviour
    {
        #region 关联的组件
        /// <summary>角色状态数据</summary>
        [HideInInspector]
        public HeroStatus chStatus = null;
        /// <summary>动画参数</summary> 
        [HideInInspector]
        public AnimationParameters animParams = null;
        /// <summary> 角色系统(技能系统的外观类)</summary>
        public CharacterSkillSystem chSystem = null;
        //移动组件
        public CharacterMotor characterMotor;
        //动画组件
        public Animator anim;

        #endregion

        #region 数据(AI系统需要的所有数据)

        /// <summary>状态对象库,保存AI所有状态</summary>
        private List<FSMState> allState;
        /// <summary>AI 关注的目标</summary>
        [HideInInspector]
        public Transform targetEnemy = null;
        [HideInInspector]
        public Transform singleTarget = null;//记录上次攻击对应的目标
        /// <summary>指令地点</summary>
        [HideInInspector]
        public Vector3 OrderPoint;
        [HideInInspector]
        public Vector3 targetDir;
        [HideInInspector]
        public float targetDis;
        /// <summary>状态机配置文件名称</summary>
        public string AIConfigFile;
        [HideInInspector]
        public Vector3 DefaultPoint;

        public Stack<SkillData> UsedSkill;

        #endregion  


        #region 如果用户有自己的运动方法，请将动动方法关联以下委托
        //行为代理
        public delegate void MovementHandler(Vector3 targetPos, float speed, float rotSpeed);
        public MovementHandler CustomeMovement = null;

        #endregion

        #region 初始状态，当前状态

        //当前状态
        private FSMState currentState;
        public FSMStateID currentSateId;
        //默认状态
        private FSMState defaultState;
        public FSMStateID defaultStateId = FSMStateID.Idle;


        /// <summary>初始化起始状态</summary>
        private void InitDefaultState()
        {
            defaultState = allState.Find(p => p.stateId == defaultStateId);
            currentState = defaultState;
            currentSateId = currentState.stateId;
            currentState.EnterState(this);
        }

        #endregion

        #region 初始化

        public void Awake()
        {
            allState = new List<FSMState>();

        }

        public void Start()
        {
            UsedSkill = new Stack<SkillData>();
            //SkillData test = GetComponent<CharacterSkillManager>().skills[1];
            //UsedSkill.Push(test);
            DefaultPoint = new Vector3(100, 100, 100);
            OrderPoint = DefaultPoint;
            characterMotor = GetComponent<CharacterMotor>();
            anim = GetComponent<Animator>();
            chStatus = GetComponent<HeroStatus>();
            chSystem = GetComponent<CharacterSkillSystem>();
            ConfigFSM();
            //configfsm();
            //初始化起始状态
            InitDefaultState();
        }

        #endregion

        #region 状态机实时工作 当前状态Action,Reason

        public void Update()
        {
            CalculateDir();
            if (currentState != null)
            {
                //当前状态下的行为
                currentState.Action(this);
                //当前状态下转换条件的检测
                currentState.Reason(this);
            }
        }

        private void CalculateDir()
        {
            if (targetEnemy != null)
            {
                targetDir = new Vector3(targetEnemy.position.x,transform.position.y,targetEnemy.position.z) - transform.position;
                //targetDir = new Vector3(targetDir.x, transform.position.y, targetDir.z);
                targetDis = Vector3.Distance(transform.position, new Vector3(targetEnemy.position.x, transform.position.y, targetEnemy.position.z));
            }
            else
            {
                targetDir = DefaultPoint;
                targetDis = -1;
            }
        }

        #endregion

        #region 状态机管理状态行为 添加 删除 状态切换 配置状态机

        /// <summary>添加状态</summary>
        public void AddState(FSMState state)
        {
            if (!allState.Exists(p => p.stateId == state.stateId))
            {
                allState.Add(state);
            }
        }

        /// <summary>删除状态</summary> 
        public void DeleteState(FSMState state)
        {
            if (allState.Exists(p => p.stateId == state.stateId))
            {
                allState.Remove(state);
            }
        }

        /// <summary>状态切换</summary> 
        public void ChangeActiveState(FSMTriggerID triggerId)
        {
            if (currentState == null) return;
            //根据条件编号，在当前状态查找输出状态
            var stateId = currentState.GetOutPutState(triggerId);
            //可能得到的3个结果  
            //1.None : 不处理
            if (stateId == FSMStateID.None) return;
            //退出当前状态
            currentState.ExitState(this);
            //2.默认状态: 将原默认状态设为当前状态
            if (stateId == FSMStateID.Default)
                currentState = defaultState;
            else //3.具体状态: 将具体状态设为当前状态
                currentState = allState.Find(p => p.stateId == stateId);

            currentSateId = currentState.stateId;
            //进入新状态
            currentState.EnterState(this);
        }

        ///// <summary>配置状态机 硬编码</summary>
        public virtual void configfsm()
        {
            //创建状态对象
            //待机
            IdleState idle = new IdleState();
            //设置转换
            idle.AddTrigger(FSMTriggerID.NoHealth, FSMStateID.Dead);
            idle.AddTrigger(FSMTriggerID.GetOrder, FSMStateID.Move);
            idle.AddTrigger(FSMTriggerID.GetTarget, FSMStateID.Pursuit);
            idle.AddTrigger(FSMTriggerID.UseSkill, FSMStateID.Skill);
            idle.AddTrigger(FSMTriggerID.ReachTarget, FSMStateID.Attacking);
            idle.AddTrigger(FSMTriggerID.IsPrompt, FSMStateID.Prompt);
            idle.AddTrigger(FSMTriggerID.IsControl, FSMStateID.Control);

            DeadState dead = new DeadState();
            MoveState move = new MoveState();
            move.AddTrigger(FSMTriggerID.ReachOrder, FSMStateID.Idle);
            move.AddTrigger(FSMTriggerID.GetTarget, FSMStateID.Pursuit);
            move.AddTrigger(FSMTriggerID.NoHealth, FSMStateID.Dead);
            move.AddTrigger(FSMTriggerID.UseSkill, FSMStateID.Skill);
            move.AddTrigger(FSMTriggerID.ReachTarget, FSMStateID.Attacking);
            move.AddTrigger(FSMTriggerID.IsPrompt, FSMStateID.Prompt);
            move.AddTrigger(FSMTriggerID.IsControl, FSMStateID.Control);

            PursuitState pursuit = new PursuitState();
            pursuit.AddTrigger(FSMTriggerID.NoHealth, FSMStateID.Dead);
            pursuit.AddTrigger(FSMTriggerID.GetOrder, FSMStateID.Move);
            pursuit.AddTrigger(FSMTriggerID.ReachTarget, FSMStateID.Attacking);
            pursuit.AddTrigger(FSMTriggerID.UseSkill, FSMStateID.Skill);
            pursuit.AddTrigger(FSMTriggerID.IsPrompt, FSMStateID.Prompt);
            pursuit.AddTrigger(FSMTriggerID.IsControl, FSMStateID.Control);

            AttackingState attack = new AttackingState();
            attack.AddTrigger(FSMTriggerID.GetOrder, FSMStateID.Move);
            attack.AddTrigger(FSMTriggerID.NotReachTarget, FSMStateID.Pursuit);
            attack.AddTrigger(FSMTriggerID.NoHealth, FSMStateID.Dead);
            attack.AddTrigger(FSMTriggerID.EnemyDie, FSMStateID.Idle);
            attack.AddTrigger(FSMTriggerID.UseSkill, FSMStateID.Skill);
            attack.AddTrigger(FSMTriggerID.IsPrompt, FSMStateID.Prompt);
            attack.AddTrigger(FSMTriggerID.IsControl, FSMStateID.Control);

            SkillState skill = new SkillState();
            skill.AddTrigger(FSMTriggerID.GetOrder, FSMStateID.Move);
            skill.AddTrigger(FSMTriggerID.SkillToPursuit, FSMStateID.Pursuit);
            skill.AddTrigger(FSMTriggerID.SkillToAttack, FSMStateID.Attacking);
            skill.AddTrigger(FSMTriggerID.EnemyDie, FSMStateID.Idle);
            skill.AddTrigger(FSMTriggerID.NoHealth, FSMStateID.Dead);
            skill.AddTrigger(FSMTriggerID.IsControl, FSMStateID.Control);

            PromptState prompt = new PromptState();
            prompt.AddTrigger(FSMTriggerID.PromptToPursuit, FSMStateID.Pursuit);
            prompt.AddTrigger(FSMTriggerID.PromptToIdle, FSMStateID.Idle);
            prompt.AddTrigger(FSMTriggerID.PromptToAttack, FSMStateID.Attacking);
            prompt.AddTrigger(FSMTriggerID.NoHealth, FSMStateID.Dead);
            prompt.AddTrigger(FSMTriggerID.IsControl, FSMStateID.Control);

            ControlState con = new ControlState();
            con.AddTrigger(FSMTriggerID.UnControl, FSMStateID.Idle);
            con.AddTrigger(FSMTriggerID.NoHealth, FSMStateID.Dead);


            AddState(idle);
            AddState(dead);
            AddState(move);
            AddState(pursuit);
            AddState(attack);
            AddState(skill);
            AddState(prompt);
            AddState(con);
        }


        /// <summary>配置状态机，通过AI配置文件配置状态机</summary>
        public void ConfigFSM()
        {

            if (string.IsNullOrEmpty(AIConfigFile)) return;
            //加载配置文件
            var dic = AIConfigLoader.Load(AIConfigFile);
                if (dic.Count == 0) return;
            //遍历所有的主键
            foreach (var mainKey in dic.Keys)
            {
                //主键名即是状态类的名称，
                var stateClassName = "AI.FSM." + mainKey;
                //反射创建状态对象
                var stateObj = Activator.CreateInstance(Type.GetType(stateClassName)) as FSMState;
                //再遍历主键下所有的子键
                foreach (var subKey in dic[mainKey].Keys)
                {
                    //子键名即是TriggerID，将字符串的TriggerID转为枚举
                    var triggerId = (FSMTriggerID)Enum.Parse(typeof(FSMTriggerID), subKey);
                    //取得子键对应的子值，子值即是StateId，//将字符串的StateId转为枚举，
                    var stateId = (FSMStateID)Enum.Parse(typeof(FSMStateID), dic[mainKey][subKey]);
                    //调用创建好的状态对象的AddTriggers方法加入triggerid,Stateid
                    stateObj.AddTrigger(triggerId, stateId);
                }
                //将创建好的状态对象放入状态机
                AddState(stateObj);
            }

        }

        #endregion



        /// <summary>播放动画</summary>
        public void PlayAnimation(string animName,bool state)
        {
            anim.SetBool(animName, state);
        }
        public bool IsUsingSkill()
        {
            return chStatus.IsUsingSkill();
        }
        public bool IsPromptSkill()
        {
            return chStatus.IsPromptSkill();
        }
        public bool IsAttacking()
        {
            return chStatus.IsAttacking();
        }
        public bool IsControl()
        {
            return chStatus.IsControl();
        }

        /// <summary>停止状态机</summary>
        public void Stop()
        {
            this.enabled = false;
        }
        public void SetOrderPoint(Vector3 point)
        {
            targetEnemy = null;
            OrderPoint = point;
        }
        public void SetTargetEnemy(Transform enemy)
        {
            OrderPoint = DefaultPoint;
            targetEnemy = enemy;
        }
    }
}
