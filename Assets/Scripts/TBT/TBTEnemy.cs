using UnityEngine;
using System.Collections;
using AI.FSM;
using System.Collections.Generic;

public class TBTEnemy : MonoBehaviour
{
    private TBTActionNode tbtnode;
    [HideInInspector]
    public EnemyStatus chStatus = null;
    [HideInInspector]
    public AnimationParameters animParams = null;
    /// <summary> 角色系统(技能系统的外观类)</summary>
    [HideInInspector]
    public CharacterSkillSystem chSystem = null;
    [HideInInspector]
    public CharacterSkillManager chSmanag = null;
    //移动组件
    public Motor enemyMotor;
    //动画组件
    public Animator anim;
    public bool isFighting;
    public Transform currentTarget;
    public int nextSkillID=0;
    //[HideInInspector]
    public float targetDis;
    [HideInInspector]
    public Vector3 DefaultPoint;
    [HideInInspector]
    public Vector3 targetDir;
    public float deg;
    public Dictionary<Transform, int> Hatreddic;
    public GameObject[] heros;
    public float startFightTime;
    public float FightTime;
    public float startPart2Time;
    public float FtTime=20;
    public float BoomTime = 0;
    public bool isAllDie = false;
    public bool isAlive = true;
    public bool isGameOver = false;

    private void Start()
    {
        Hatreddic = new Dictionary<Transform, int>();
        enemyMotor = GetComponent<Motor>();
        anim = GetComponent<Animator>();
        chStatus = GetComponent<EnemyStatus>();
        chSystem = GetComponent<CharacterSkillSystem>();
        chSmanag = GetComponent<CharacterSkillManager>();
         isFighting = false;
        heros= GameObject.FindGameObjectsWithTag("Hero");
        foreach(var hero in heros)
        {
            Hatreddic.Add(hero.transform, 0);
            hero.GetComponent<HeroStatus>().onDead = OnHeroDead;
        }
        chStatus.OnDamage += OnDamage;
        var sel = new TBTSelectorNode();
        sel.AddChild(new TBTSequenceNode().SetPrecondition(new IsOnFighting()).AddChild(new SearchEnemy()).AddChild(new SetTarget())).AddChild(new SetTarget());
        var seq = new TBTSequenceNode();
        seq.AddChild(sel);
        var selskill = new TBTSelectorNode();
        selskill.AddChild(new GetPart1CurrentSkill().SetPrecondition(new IsHpHIGH())).AddChild(new GetPart2CurrentSkill());
        var par = new TBTParallelNode();
        par.AddChild(selskill).AddChild(new TurnTOTarget()).AddChild(new MoveToTarget());
        var rootfight = new TBTSequenceNode();
        rootfight.SetPrecondition(new IsAllHeroDie()).AddChild(seq).AddChild(par).AddChild(new AttackUseSkill());
        var root = new TBTSelectorNode();
        root.AddChild(new Dead().SetPrecondition(new IsDead())).AddChild(rootfight).AddChild(new Idel());
        tbtnode = root;
    }
    private void Update()
    {
        CalculateDirTime();
        if (tbtnode.Enter(this))
        {
            tbtnode.Update(this);
        }
        else
        {
            tbtnode.Exit(this);
        }
    }
    private void CalculateDirTime()
    {
        if (currentTarget != null)
        {
            targetDir = new Vector3(currentTarget.position.x, transform.position.y, currentTarget.position.z) - transform.position;
            targetDis = Vector3.Distance(transform.position, new Vector3(currentTarget.position.x, transform.position.y, currentTarget.position.z));
        }
        else
        {
            targetDir = DefaultPoint;
            targetDis = -1;
        }
        if (startFightTime != 0)
            FightTime = Time.time - startFightTime;
        deg = Mathf.Abs(Vector3.Angle(transform.forward,targetDir));
    }
    public void ChangeHatred(Transform hero,int value)
    {
        Hatreddic[hero] = value;
    }
    public Transform GetMaxHatred()
    {
        Transform maxHatred=null;
        int i = -1;
        foreach(var hero in heros)
        {
            if (Hatreddic[hero.transform] > i)
            {
                maxHatred = hero.transform;
                i = Hatreddic[hero.transform];
            }
        }
        return i==0?null:maxHatred;
    }
    public void OnFight()
    {
        isFighting = true;
        startFightTime = Time.time;
        foreach(var hero in heros)
        {
            Hatreddic[hero.transform] = 1;
        }
        UIManager.Instance.GetUI<UITime>(UIName.UITime).SetVisible(true);
        UIManager.Instance.GetUI<UITime>(UIName.UITime).StartTick();
    }
    public bool IsOutofRange()
    {
        if (nextSkillID != 0&& nextSkillID!=1)
            return targetDis >= chSmanag.GetSkill(nextSkillID).attackDistance;
        else
            return targetDis >=chStatus.attackDistance;
    }
    private void OnHeroDead()
    {
        var isalldie = true;
        foreach(var hero in heros)
        {
            if (hero.GetComponent<HeroStatus>().HP > 0)
            {
                isalldie = false;
            }
            else
                Hatreddic[hero.transform] = 0;
        }
        isAllDie = isalldie;
    }
    private void OnDamage(CharacterStatus damager)
    {
        if (startFightTime == 0)
        {
            isFighting = true;
            OnFight();
        }
        if(damager.chType==CharacterType.MT)
            Hatreddic[damager.transform] += 2;
        else
            Hatreddic[damager.transform]++;
    }

}
