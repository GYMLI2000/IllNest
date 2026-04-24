using UnityEngine;

public class BipolarIdleState : IdleState
{
    private new BipolarEnemy enemy;
    public BipolarIdleState(BipolarEnemy enemy) : base(enemy)
    {
        this.enemy = enemy;
    }

    public override State ChangeState()
    {
        if (enemy.target != null && !enemy.isEnraged)
        {
            return enemy.attackState;
        }
        else if (enemy.target != null && enemy.isEnraged)
        {
            return enemy.attackStateManic;
        }


        return enemy.idleState;
    }
}
