using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange; 
    public float skillRange;
    public float coolDown;
    public int minDamge;
    public int maxDamage;
    public float criticalMultiplier;
    public float criticalChance;

    public void ApplyWeaponData(AttackData_SO weapon)
    {
        attackRange = weapon.attackRange;
        skillRange = weapon.skillRange;
        coolDown = weapon.coolDown;
        minDamge = weapon.minDamge;
        maxDamage = weapon.maxDamage;
        criticalChance = weapon.criticalChance;
        criticalMultiplier = weapon.criticalMultiplier;

    }


}
