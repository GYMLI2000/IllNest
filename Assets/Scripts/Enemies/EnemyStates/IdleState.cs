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
        if (enemy.target != null)
        {
            return new ChaseState(enemy);
        }

        return this;
    }

    public override void FixedAI()
    {
        
    }
}
