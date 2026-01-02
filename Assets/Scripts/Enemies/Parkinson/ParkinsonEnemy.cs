using System.Collections.Generic;
using UnityEngine;

public class ParkinsonEnemy : Enemy
{
    protected override void SetPoolKeys()
    {
        poolKey = "ParkinsonEnemy";
        projKey = "ParkinsonProjectile";

    }

    protected override void InitializeStats()
    {
        killParticleColor = new Color(168f / 255f, 137f / 255f, 68f /255f, 0.2f);
        chaseRange =  3;
        damage = 1;
        speed = 7;
        health = 5;
        isAttacking = false;
        attackCooldown = 2;
        attackDuration = 0.5f;
        chargeTime = 0.6f;
        lastAttack = Time.time;
        attackState = new ParkinsonAttackState(this);
        idleState = new IdleState(this);
        fleeState = new FleeState(this);
        knockback = 1f;
    }
}
