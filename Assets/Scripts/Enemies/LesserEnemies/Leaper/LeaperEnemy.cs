using System.Collections.Generic;
using UnityEngine;

public class LeaperEnemy : Enemy
{
    public float dashPower { get; private set; }
    public Vector2 dashDirection;

    protected override void SetPoolKeys()
    {
        poolKey = "LeaperEnemy";
    }

    protected override void InitializeStats()
    {
        killParticleColor = new Color(122f/255f, 107f/255f, 227f/255f, 0.2f);
        chaseRange =  100000;
        damage = 1;
        health = 4;
        isAttacking = false;
        attackCooldown = 0;
        attackDuration = .3f;
        chargeTime = 1f;
        lastAttack = Time.time;
        attackState = new LeaperAttackState(this);
        chaseState = new ChaseState(this);
        idleState = new IdleState(this);
        dashPower = 10;
        knockback = 2f;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isAttacking && (collision.gameObject.CompareTag("Wall") ||  collision.gameObject.CompareTag("Player")))
        {
            dashDirection = dashDirection/3 * -1;
        }
    }
}
