using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GUARD, PATROL, CHASE, DEAD }
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour,IEndGameObserver
{
    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    private Animator anim;

    [Header("Basic Settings")]
    public float sightRadius;
    public bool isGuard;
    private float speed;//��¼ԭ�����ٶ�
    private GameObject attackTarget;
    public float lookAtTime;
    private float remainLookAtTime;

    //Ѳ��
    [Header("Patrol State")]
    public float patrolRange;
    private Vector3 wayPoint;
    private Vector3 guardPos;

    //bool��϶���
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool playDead;

   

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
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
        //�ϰͲ�Ҫɾ��if
        if (!playDead)
        {
            SwitchStates();
            SwitchAnimation();
        }
    }

    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
    }
    void SwitchStates()
    {
        //�������player �л���CHASE
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
        //TODO:׷player���ڹ�����Χ���򹥻�����϶���
        if(!FoundPlayer())
        {
            //TODO:���ѻص���һ��״̬
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

    //�յ��㲥��Ĵ�������
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

        //�ж��Ƿ������Ѳ�ߵ�
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

    void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;

        float randomX = UnityEngine.Random.Range(-patrolRange, patrolRange);
        float randomZ = UnityEngine.Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x + randomX,transform.position.y, guardPos.z + randomZ);
        //FIXME�����ܳ��ֵ�����,�������ж�������ǲ������߹�ȥ�ĵ�
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint,out hit,patrolRange,1)? hit.position:transform.position;
    }
    //ѡ������ʱ�Żử������ѵ����Χ
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }
}
