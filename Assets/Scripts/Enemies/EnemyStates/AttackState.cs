using UnityEngine;

public abstract class AttackState : State
{
    
    public AttackState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public override void AI()
    {
        if (enemy.isAttacking && enemy.lastAttack + enemy.attackDuration <= Time.time)
        {
            enemy.isAttacking = false;
        }


        if (!enemy.isAttacking && enemy.attackCooldown + enemy.lastAttack <= Time.time )
        {
            enemy.isAttacking = true;
            enemy.lastAttack = Time.time;

            Attack();
        }
    }

    public abstract void Attack();

    public override State ChangeState()
    {
        if (enemy.target == null && !enemy.isAttacking)
        {
            return enemy.idleState;
        }
        if (Vector2.Distance(enemy.transform.position, enemy.target.transform.position) > enemy.chaseRange && !enemy.isAttacking)
        {
            return new ChaseState(enemy);
        }
        return this;
    }

    public override void FixedAI()
    {
        
    }
}
