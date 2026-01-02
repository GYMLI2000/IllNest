using System.Collections.Generic;
using UnityEngine;

public class ADHDDistraction : Enemy
{
    public override void EnableEnemy()
    {
        InitializeStats();
        base.EnableEnemy();
    }

    protected override void InitializeStats()
    {
        killParticleColor = new Color(50f / 255f, 227f / 255f, 50f /255f, 0.2f);
        damage = 1;
        speed = 5;
        health = 1;
        poolKey = "ADHDDistraction";
        isAttacking = false;
        attackCooldown = 0.5f;
        attackDuration = .2f;
        lastAttack = Time.time;
        idleState = new ADHDDistractionState(this);
        knockback = 5f;
    }
}
