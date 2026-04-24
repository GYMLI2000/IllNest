using NUnit.Framework.Constraints;
using UnityEngine;

public class OCDChaseState : ChaseState
{
    private new OCDEnemy enemy;

    public OCDChaseState(OCDEnemy enemy) : base(enemy)
    {
        this.enemy = enemy;
    }

    public override State ChangeState()
    {
        if (enemy.target == null)
        {
            return enemy.idleState;
        }

        float dx = Mathf.Abs(enemy.target.transform.position.x - enemy.transform.position.x);
        float dy = Mathf.Abs(enemy.target.transform.position.y - enemy.transform.position.y);

        if (!enemy.isAttacking && !enemy.isCharging && (dx <= 0.2f || dy <= 0.2f))
        {
            enemy.axisLock = -1; 
            return enemy.attackState;
        }

        return this;
    }

    public override void AI()
    {
        if (enemy.rb.linearVelocity.magnitude > 0)
        {
            enemy.animator.SetBool("isWalking", true);
        }
        else
        {
            enemy.animator.SetBool("isWalking", false);
        }

        Vector2 direction = enemy.target.transform.position - enemy.transform.position;
        float tolerance = 0.1f;


        if (enemy.axisLock == -1)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                enemy.axisLock = 0;
            else
                enemy.axisLock = 1; 
        }


        if (enemy.axisLock == 0)
        {
            moveDir = new Vector2(Mathf.Sign(direction.x), 0);


            if (Mathf.Abs(direction.x) <= tolerance)
                enemy.axisLock = -1;
        }

        else if (enemy.axisLock == 1)
        {
            moveDir = new Vector2(0, Mathf.Sign(direction.y));

            if (Mathf.Abs(direction.y) <= tolerance)
                enemy.axisLock = -1;
        }
    }

}
