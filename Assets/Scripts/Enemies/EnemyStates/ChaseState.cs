using UnityEngine;

public class ChaseState : State
{
    private Enemy enemy;
    private Rigidbody2D rb;
    private float speed;

    public ChaseState(Enemy enemy)
    {
        this.enemy = enemy;
        this.speed = enemy.speed;
        rb = enemy.GetComponent<Rigidbody2D>();

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
        if (Vector2.Distance(enemy.transform.position,enemy.target.transform.position) < enemy.chaseRange)
        {
            return new AttackState(enemy);
        }
        return this;
    }

    public override void FixedAI()
    {
        if (enemy.target == null) return;

        Vector2 moveDir = (enemy.target.transform.position - enemy.transform.position).normalized;


        rb.MovePosition(rb.position +moveDir * speed * Time.deltaTime);

        Debug.Log("Movin");
    }
}
