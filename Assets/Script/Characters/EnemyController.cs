using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GUARD, PATROL, CHASE, DEAD }
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    private Animator anim;

    [Header("Basic Settings")]
    public float sightRadius;
    public bool isGuard;
    private float speed;//记录原来的速度
    private GameObject attackTarget;

    //bool配合动画
    bool isWalk;
    bool isChase;
    bool isFollow;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        speed = agent.speed;
    }

    void Update()
    {
        SwitchStates();
        SwitchAnimation();
    }

    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
    }
    void SwitchStates()
    {
        //如果发现player 切换到CHASE
        if(FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }

        switch(enemyStates)
        {
            case EnemyStates.GUARD:
                break;
            case EnemyStates.PATROL:
                break;
            case EnemyStates.CHASE:
                ChaseAction();
                break;
            case EnemyStates.DEAD:
                break;
        }
    }

     void ChaseAction()
    {
        isWalk = false;
        isChase = true;
        agent.speed = speed;
        //TODO:追player，在攻击范围内则攻击，配合动画
        if(!FoundPlayer())
        {
            //TODO:拉脱回到上一个状态
            isFollow = false;
            agent.destination = transform.position;
        }
        else
        {
            isFollow = true;
            agent.destination = attackTarget.transform.position;
        }
    }

    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);

        foreach (var target in colliders)
        {
            if(target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }
   

}
