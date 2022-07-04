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
    public EnemyStates enemyStates;
    private NavMeshAgent agent;
    protected Animator anim;
    private Collider coll;
    protected CharacterStats characterStats;

    [Header("Basic Settings")]
    public float sightRadius;
    public bool isGuard;
    private float speed;
    protected GameObject attackTarget;
    public float lookAtTime;
    private float remainLookAtTime;
    protected float lastAttackTime;
    private Quaternion guardRotation;


    [Header("Patrol State")]
    public float patrolRange;
    private Vector3 wayPoint;
    private Vector3 guardPos;

    public GameObject Partol;


     bool isWalk;
     bool isChase;
     bool isFollow;
    protected bool playerDead;
    public bool isDead;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();

        speed = agent.speed;
        guardPos = transform.position;
        guardRotation = transform.rotation;
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
        //³¡¾°ÇÐ»»ºóÐÞ¸Äµô
        GameManager.Instance.AddObserver(this);
    }

    //void OnEnable()
    //{
    //    GameManager.Instance.AddObserver(this);
    //}

    void OnDisable()
    {
        if (!GameManager.IsInitialized) { return; }
        GameManager.Instance.RemoveObserver(this);

        if (GetComponent<LootSpawner>() && isDead)
            GetComponent<LootSpawner>().Spawnloot();

        if (QuestManager.IsInitialized && isDead)
            QuestManager.Instance.UpdateQuestProgress(this.name, 1);
    }

   protected virtual void Update()
    {
        if(characterStats.CurrentHealth == 0)
        {
            Partol.SetActive(true);
            isDead = true;
        }
            
        if (!playerDead)
        {
            SwitchStates();
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime;
        }

    }

    protected virtual void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death",isDead);
    }
   protected void SwitchStates()
    {
        if(isDead)
        enemyStates = EnemyStates.DEAD;
        else if(FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }

        switch(enemyStates)
        {
            case EnemyStates.GUARD:
                GuardAction();
                break;
            case EnemyStates.PATROL:
                PatrolAction();
                break;
            case EnemyStates.CHASE:
                ChaseAction();
                break;
            case EnemyStates.DEAD:
                DeadAction();
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
        //TODO:ï¿½Ú¹ï¿½ï¿½ï¿½ï¿½ï¿½Î§ï¿½ï¿½ï¿½ò¹¥»ï¿½
        if(TargetInAttackRange() || TargetInSkillRange())
        {
            isFollow = false;
            agent.isStopped = true;

            if(lastAttackTime < 0)
            {
                lastAttackTime = characterStats.attackData.coolDown;

                //ï¿½ï¿½ï¿½ï¿½ï¿½Ð¶ï¿½
                characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;
                //Ö´ï¿½Ð¹ï¿½ï¿½ï¿½
                Attack();

            }
        }

    }

    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if(TargetInAttackRange())
        {
            //ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½
            anim.SetTrigger("Attack");
        }
        if(TargetInSkillRange())
        {
            //ï¿½ï¿½ï¿½Ü¹ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½
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

        anim.SetBool("Win", true);
        playerDead = true;
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

        void DeadAction()
        {
        coll.enabled = false;
        agent.radius = 0;
        Destroy(gameObject,2f);
        }
    void GuardAction()
    {
        isChase = false;
        if(transform.position != guardPos)
        {
             isWalk = true;
             agent.isStopped = false;
             agent.destination = guardPos;
             if(Vector3.SqrMagnitude(guardPos-transform.position) <= agent.stoppingDistance)
             {
                 
                 isWalk = false;
                transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
            }
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
    
    //Animation Event
    void Hit()
    {
        if(attackTarget != null && transform.isFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }
}
