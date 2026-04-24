using UnityEngine;

public class BipolarChaseState : ChaseState
{
    private new BipolarEnemy enemy;

    public BipolarChaseState(BipolarEnemy enemy) : base(enemy)
    {
        this.enemy = enemy;
    }

    public override State ChangeState()
    {
        if (enemy.target == null)
        {
            return enemy.idleState;
        }
        if (!enemy.isAttacking && !enemy.isCharging && !enemy.isEnraged)
        {
            return enemy.attackState;
        }
        
        return this;
    }
}
