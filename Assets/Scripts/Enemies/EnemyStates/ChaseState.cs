using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

public class ChaseState : State
{
    protected Rigidbody2D rb;
    protected float speed;
    protected Vector2 moveDir;

    public ChaseState(Enemy enemy)
    {
        this.enemy = enemy;
        this.speed = enemy.speed;
        rb = enemy.rb;

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
        moveDir = (enemy.target.transform.position - enemy.transform.position).normalized;

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



        enemy.rb.linearVelocity = Vector2.Lerp(enemy.rb.linearVelocity, moveDir * speed, 0.1f);
        enemy.rb.position += enemy.rb.linearVelocity * Time.deltaTime;

        //rb.linearVelocity = moveDir * speed;


    }
}
