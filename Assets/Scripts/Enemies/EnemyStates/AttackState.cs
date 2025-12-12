using UnityEngine;

public abstract class AttackState : State
{
    
    public AttackState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public override void AI()
    {
        // Když dokonèil útok pøepnout zpìt
        if (enemy.isAttacking && enemy.lastAttack + enemy.attackDuration <= Time.time)
        {
            enemy.isAttacking = false;
            enemy.animator.SetTrigger("Idle");
        }

        // ==== 1) Zaèátek charging ====
        if (!enemy.isAttacking && !enemy.isCharging &&
            enemy.lastAttack + enemy.attackCooldown <= Time.time)
        {

            enemy.isCharging = true;
            enemy.chargeStart = Time.time;
            enemy.animator.SetTrigger("Charge");

            OnCharge(); // vizuální efekty
            return;
        }

        // ==== 2) Bìhem charge ====
        if (enemy.isCharging)
        {
            if (enemy.chargeStart + enemy.chargeTime <= Time.time)
            {
                // Charge dokonèen  spustit útok
                enemy.isCharging = false;
                enemy.isAttacking = true;
                enemy.lastAttack = Time.time;
                enemy.animator.SetTrigger("Attack");


                Attack();
            }

            return; // nic jiného bìhem charge
        }

    }

    public abstract void Attack();

    public virtual void OnCharge()
    {

    }

    public override State ChangeState()
    {
        if (enemy.target == null && !enemy.isAttacking && !enemy.isCharging)
        {
            return enemy.idleState;
        }
        if (enemy.chaseState != null && Vector2.Distance(enemy.transform.position, enemy.target.transform.position) > enemy.chaseRange && !enemy.isAttacking && !enemy.isCharging)
        {
            return enemy.chaseState;
        }
        if (enemy.fleeState != null && Vector2.Distance(enemy.transform.position, enemy.target.transform.position) < enemy.chaseRange && !enemy.isAttacking && !enemy.isCharging)
        {
            return enemy.fleeState;
        }

        return this;
    }

    public override void FixedAI()
    {
        
    }
}
