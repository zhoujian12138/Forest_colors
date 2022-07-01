using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : EnemyController
{

    [Header("Skill")]

    public float kickForce = 15;
    public RuntimeAnimatorController animControl;
    private bool reLife = true;
    private bool stopAgent = true;
    private bool bossDie = true;

    //Animation Event
    public void KickOff()
    {
        if (attackTarget != null)
        {
            transform.LookAt(attackTarget.transform);

            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();

            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
        }
    }
    protected override void Update()
    {
        if (reLife && characterStats.CurrentHealth <= (int)(characterStats.MaxHealth / 2))//boss是第一条命
        {
            stopAgent = false;
            reLife = false;
            anim.SetTrigger("Wait");
            Invoke("WaitBoss", 5.0f);
            Invoke("BossBack", 10.0f);
        }
        if (characterStats.CurrentHealth == 0 && bossDie)
        {
            stopAgent = false;
            bossDie = false;
            Partol.SetActive(true);
            isDead = true;
            anim.SetTrigger("Wait");
            Invoke("BossDie", 1.0f);
            anim.SetTrigger("Dizzy");
            Invoke("BossDie", 3.0f);
            anim.SetTrigger("Die");
            Invoke("BossDie", 6.0f);
            Invoke("BossDestroy", 7.0f);
        }
        if (!playerDead && stopAgent)
        {
            SwitchStates();
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime;
        }
    }
    void WaitBoss()
    {
        anim.SetTrigger("Violent");
        characterStats.CurrentHealth = characterStats.MaxHealth;
    }
    void BossBack()
    {
        anim.runtimeAnimatorController = animControl;//更换动画
        stopAgent = true;
    }
  void BossDie()
    {

    }
   void BossDestroy()
    {
        stopAgent = true;
    }
}
