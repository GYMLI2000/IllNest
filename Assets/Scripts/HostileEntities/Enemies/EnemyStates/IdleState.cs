using UnityEngine;

public class IdleState : State
{

    public IdleState(Enemy enemy) 
    {
        this.enemy = enemy;
    }


    public override void AI()
    {

    }

    public override State ChangeState()
    {
        if (enemy.target != null && enemy.chaseState != null)
        {
            return enemy.chaseState;
        }
        else if (enemy.target != null && enemy.fleeState != null)
        {
            return enemy.attackState;
        }


        return enemy.idleState;
    }

    public override void FixedAI()
    {
        
    }
}
