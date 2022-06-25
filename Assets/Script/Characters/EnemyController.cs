using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GUARD, PATROL, CHASE, DEAD }
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour,IEndGameObserver
{
    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    private Animator anim;
    private CharacterStats characterStats;

    [Header("Basic Settings")]
    public float sightRadius;
    public bool isGuard;
    private float speed;
    private GameObject attackTarget;
    public float lookAtTime;
    private float remainLookAtTime;
    private float lastAttackTime;

  
    [Header("Patrol State")]
    public float patrolRange;
    private Vector3 wayPoint;
    private Vector3 guardPos;

   
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool playDead;

   

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();

        speed = agent.speed;
        guardPos = transform.position;
        remainLookAtTime = lookAtTime;
    }

    void Start()
    {
        if(isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();
        }
        GameManager.Instance.AddObserver(this);
        Debug.Log(characterStats.attackData.coolDown);
    }

    //void OnEnable()
    //{
    //    GameManager.Instance.AddObserver(this);
    //}

    void OnDisable()
    {
        if (!GameManager.IsInitialized) { return; }
        GameManager.Instance.RemoveObserver(this);
    }

    void Update()
    {
        if (!playDead)
        {
            SwitchStates();
            SwitchAnimation();
        }
        SwitchStates();
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }

    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
    }
    void SwitchStates()
    {
        
        if(FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }

        switch(enemyStates)
        {
            case EnemyStates.GUARD:
            isChase = false;
            if(transform.position != guardPos)
            {
                isWalk = true;

            }
                break;
            case EnemyStates.PATROL:
                PatrolAction();
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
        if(!FoundPlayer()) 
        {
            isFollow = false;
            if (remainLookAtTime > 0)
            {
                agent.destination = transform.position;
                remainLookAtTime -= Time.deltaTime;
            }

            else if (isGuard)
                enemyStates = EnemyStates.GUARD;
            else
                enemyStates = EnemyStates.PATROL;
            
        }
        else
        {
            isFollow = true;
            agent.isStopped = false;
            agent.destination = attackTarget.transform.position;
        }
        //TODO:ÔÚ¹¥»÷·¶Î§ÄÚÔò¹¥»÷
        if(TargetInAttackRange() || TargetInSkillRange())
        {
            isFollow = false;
            agent.isStopped = true;

            if(lastAttackTime < 0)
            {
                lastAttackTime = characterStats.attackData.coolDown;

                //±©»÷ÅÐ¶Ï
                characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;
                //Ö´ÐÐ¹¥»÷
                Attack();

            }
        }

    }

    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if(TargetInAttackRange())
        {
            //½üÉí¹¥»÷¶¯»­
            anim.SetTrigger("Attack");
        }
        if(TargetInSkillRange())
        {
            //¼¼ÄÜ¹¥»÷¶¯»­
            anim.SetTrigger("Skill");
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

   
    public void EndNotify()
    {

        anim.SetBool("win", true);
        playDead = true;
        isChase = false;
        isWalk = false;
        attackTarget = null;
    }

    void PatrolAction()
    {
        isChase = false;
        agent.speed = speed * 0.5f;

        if(Vector3.Distance(wayPoint,transform.position) <= agent.stoppingDistance)
        {
            isWalk = false;
            if(remainLookAtTime > 0)
                remainLookAtTime -= Time.deltaTime;
            else
               GetNewWayPoint();
        }
        else
        {
            isWalk = true;
            agent.destination = wayPoint;
        }
    }

    bool TargetInAttackRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        else
            return false;
    }

    bool TargetInSkillRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        else
            return false;
    }
    void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;

        float randomX = UnityEngine.Random.Range(-patrolRange, patrolRange);
        float randomZ = UnityEngine.Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x + randomX,transform.position.y, guardPos.z + randomZ);
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint,out hit,patrolRange,1)? hit.position:transform.position;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }
}
