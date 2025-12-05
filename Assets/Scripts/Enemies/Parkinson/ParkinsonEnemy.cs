using System.Collections.Generic;
using UnityEngine;

public class ParkinsonEnemy : Enemy
{
    protected override void InitializeStats()
    {
        killParticleColor = new Color(50f / 255f, 227f / 255f, 50f /255f, 0.2f);
        chaseRange =  4;
        damage = 1;
        speed = 7;
        health = 3;
        poolKey = "ADHDEnemy";
        isAttacking = false;
        attackCooldown = 2;
        attackDuration = .5f;
        chargeTime = 1.5f;
        lastAttack = Time.time;
        attackState = new ParkinsonAttackState(this);
        idleState = new IdleState(this);
        knockback = 0f;
    }
}
