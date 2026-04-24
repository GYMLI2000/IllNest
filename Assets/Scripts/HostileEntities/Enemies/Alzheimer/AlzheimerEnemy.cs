using System;
using System.Collections.Generic;
using UnityEngine;

public class AlzheimerEnemy : Enemy
{
    public float moveCooldown {get; private set; }
    public float teleportCooldown { get; private set; }

    public float lastMove;
    public float lastShoot;
    public float lastTeleport;

    protected override void SetPoolKeys()
    {
        poolKey = "AlzheimerEnemy";
        projKey = "AlzheimerProjectile";

    }

    protected override void InitializeStats()
    {
        killParticleColor = new Color(100f / 255f, 100f / 255f, 100f /255f, 0.2f);
        chaseRange =  4;
        damage = 1;
        speed = 2;
        health = 10;
        poolKey = "AlzheimerEnemy";
        projKey = "AlzheimerProjectile";
        isAttacking = false;
        attackCooldown = 0.3f;
        moveCooldown = 2f;
        teleportCooldown = 5f;
        attackDuration = 0f;
        chargeTime = 0f;
        lastAttack = Time.time;
        lastMove = Time.time;
        lastShoot = Time.time;
        lastTeleport = Time.time;
        attackState = new AlzheimerAttackState(this);
        idleState = new AlzheimerIdleState(this);
        knockback = 2f;
        looksATarget = false;
    }


    public override void TakeDamage(int damage)
    {
        lastAttack -= 1;
        base.TakeDamage(damage);
    }
}
