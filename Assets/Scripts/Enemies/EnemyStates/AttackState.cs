using UnityEngine;

public class AttackState : State
{
    private Enemy enemy;
    public AttackState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public override void AI()
    {
        
    }

    public override State ChangeState()
    {
        if (enemy.target == null)
        {
            return new IdleState(enemy);
        }
        if (Vector2.Distance(enemy.transform.position, enemy.target.transform.position) > enemy.chaseRange)
        {
            return new ChaseState(enemy);
        }
        return this;
    }

    public override void FixedAI()
    {
        
    }
}
