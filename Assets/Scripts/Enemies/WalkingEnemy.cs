using UnityEngine;
using UnityEngine.EventSystems;

public class WalkingEnemy : Enemy
{
    protected override void AI()
    {
        
    }

    protected override void FixedAI()
    {
        if (target == null) return;

        Vector2 moveDir = (target.transform.position - transform.position).normalized;


        rb.MovePosition(rb.position +moveDir * speed * Time.deltaTime);
    }

}
