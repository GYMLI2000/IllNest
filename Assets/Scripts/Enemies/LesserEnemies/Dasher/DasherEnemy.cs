using UnityEngine;

public class DasherEnemy : Enemy
{
    public float dashPower { get; private set; }
    public Vector2 dashDirection;

    protected override void SetPoolKeys()
    {
        poolKey = "DasherEnemy";
    }

    protected override void InitializeStats()
    {
        killParticleColor = new Color(50f / 255f, 227f / 255f, 50f /255f, 0.2f);
        chaseRange =  5;
        damage = 1;
        health = 4;
        speed =5;
        isAttacking = false;
        attackCooldown = 2f;
        attackDuration = .5f;
        chargeTime = 1.5f;
        lastAttack = Time.time;
        attackState = new DasherAttackState(this);
        chaseState = new ChaseState(this);
        idleState = new IdleState(this);
        dashPower = 20;
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
