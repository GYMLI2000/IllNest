using UnityEngine;

public class DasherAttackState : AttackState
{
    private new DasherEnemy enemy;


    public DasherAttackState(DasherEnemy enemy) : base(enemy)
    {
        this.enemy = enemy;
    }

    public override void FixedAI()
    {
        if (!enemy.isAttacking)
            return;

        enemy.rb.linearVelocity = enemy.dashDirection* enemy.dashPower;
    }

    public override void AI()
    {

        base.AI();


        if (!enemy.isAttacking)
        {
            enemy.knockback = 2f;
        }
        else
        {
            enemy.knockback = 5f;
        }

    }


    public override void Attack()
    {
        enemy.dashDirection =
               (enemy.target.transform.position - enemy.transform.position).normalized;
    }

}
