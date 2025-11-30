using System.Runtime.Serialization;
using UnityEngine;

public class ChaseState : State
{
    private Rigidbody2D rb;
    private float speed;

    public ChaseState(Enemy enemy)
    {
        this.enemy = enemy;
        this.speed = enemy.speed;
        rb = enemy.rb;

    }


    public override void AI()
    {
        
    }

    public override State ChangeState()
    {
        if (enemy.target == null) 
        {
            return enemy.idleState;
        }
        if (Vector2.Distance(enemy.transform.position,enemy.target.transform.position) < enemy.chaseRange - 1) // -1 zabranuje rychlemu switchovani statetu
        {
            return enemy.attackState;
        }
        return this;
    }

    public override void FixedAI()
    {

        if (enemy.target == null) return;

        Vector2 moveDir = (enemy.target.transform.position - enemy.transform.position).normalized;


        rb.MovePosition(rb.position +moveDir * speed * Time.fixedDeltaTime);


    }
}
