using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class SpitterEnemy : Enemy
{
    protected override void SetPoolKeys()
    {
        poolKey = "SpitterEnemy";
        projKey = "SpitProjectile";
    }

    protected override void InitializeStats()
    {
        killParticleColor = new Color(50f/255f, 200f/255f, 50f/255f, 0.2f);
        chaseRange =  5;
        damage = 1;
        health = 4;
        speed =2;
        isAttacking = false;
        attackCooldown = 2f;
        attackDuration = .5f;
        chargeTime = 1.5f;
        lastAttack = Time.time;
        attackState = new SpitterAttackState(this);
        chaseState = new ChaseState(this);
        idleState = new IdleState(this);
        knockback = 2f;
    }
}
