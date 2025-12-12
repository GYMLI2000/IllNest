using UnityEngine;

public class BipolarAttackStateManic : AttackState
{
    private new BipolarEnemy enemy;

    public BipolarAttackStateManic(BipolarEnemy enemy) : base(enemy)
    {
        this.enemy = enemy;
    }

    public override void Attack()
    {
        throw new System.NotImplementedException();
    }


    public override State ChangeState()
    {
        if (enemy.target == null && !enemy.isAttacking && !enemy.isCharging)
        {
            return enemy.idleState;
        }
        if (Vector2.Distance(enemy.transform.position, enemy.target.transform.position) > enemy.chaseRange && !enemy.isAttacking && !enemy.isCharging && enemy.isEnraged)
        {
            return enemy.chaseState;
        }
        if(!enemy.isEnraged && !enemy.isAttacking && !enemy.isCharging)
        {
            return enemy.attackState;
        }

        return this;
    }


}
