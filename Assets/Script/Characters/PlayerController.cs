using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    //    //private CharacterStats characterStats;

        private GameObject attackTarget;
        private float lastAttackTime;
    //    //public bool isDead;

    //    //private float stopDistance;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        //anim = GetComponent<Animator>();
        //characterStats = GetComponent<CharacterStats>();

        //stopDistance = agent.stoppingDistance;
    }

    void OnEnable()
    {
        //MouseManager.Instance.OnMouseClicked += MoveToTarget;
        //MouseManager.Instance.OnEnemyClicked += EventAttack;
        //GameManager.Instance.RigisterPlayer(characterStats);
    }

    void Start()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;
        //SaveManager.Instance.LoadPlayerData();

    }

    private void EventAttack(GameObject target)
    {
        if(target != null)
        {
            attackTarget = target;
            StartCoroutine(MoveToAttackTarget());
        }
    }

    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;

        transform.LookAt(attackTarget.transform);

        //TODO:修改攻击范围参数
        while(Vector3.Distance(attackTarget.transform.position,transform.position)>1)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }
        
        agent.isStopped = true;
        //Attack

        if(lastAttackTime < 0)
        {
            anim.SetTrigger("Attack");
            //重置冷却时间
            lastAttackTime = 0.5f;
        }
    }

    void OnDisable()
    {
        //if (!MouseManager.IsInitialized) return;
        //MouseManager.Instance.OnMouseClicked -= MoveToTarget;
        //MouseManager.Instance.OnEnemyClicked -= EventAttack;
    }

        void Update()
        {
    //        isDead = characterStats.CurrentHealth == 0;

    //        if (isDead)
    //            GameManager.Instance.NotifyObservers();

    //        // KeyboardControl();
    //        // ActionAttack();

            SwitchAnimation();

            lastAttackTime -= Time.deltaTime;
        }

        private void SwitchAnimation()
        {
           anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
           //anim.SetBool("Death", isDead);
        }

    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();
        //if (isDead) return;

        //agent.stoppingDistance = stopDistance;
        agent.isStopped = false;
        agent.destination = target;
    }

    //  private void EventAttack(GameObject target)
    //  {
    //     if (isDead) return;

    //     if (target != null)
    //     {
    //         attackTarget = target;
    //         characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;
    //         StartCoroutine(MoveToAttackTarget());
    //     }
    // }

    //ienumerator movetoattacktarget()
    //{
    //    agent.isstopped = false;
    //    agent.stoppingdistance = characterstats.attackdata.attackrange;

    //    transform.lookat(attacktarget.transform);

    //    if (attacktarget == null)
    //        yield break;

    //    while (vector3.distance(attacktarget.transform.position, transform.position) > characterstats.attackdata.attackrange)
    //    {
    //        agent.destination = attacktarget.transform.position;
    //        yield return null;
    //    }

    //    agent.isstopped = true;
    //    //attack
    //    if (lastattacktime < 0)
    //    {
    //        anim.setbool("critical", characterstats.iscritical);
    //        anim.settrigger("attack");
    //        //重置冷却时间
    //        lastattacktime = characterstats.attackdata.cooldown;
    //    }
    //}

    //    //Animation Event
    //    void Hit()
    //    {
    //        if (attackTarget.CompareTag("Attackable"))
    //        {
    //            if (attackTarget.GetComponent<Rock>() && attackTarget.GetComponent<Rock>().rockStates == Rock.RockStates.HitNothing)
    //            {
    //                attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;
    //                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
    //                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
    //            }
    //        }
    //        else
    //        {
    //            var targetStats = attackTarget.GetComponent<CharacterStats>();

    //            targetStats.TakeDamage(characterStats, targetStats);
    //        }
    //    }
}