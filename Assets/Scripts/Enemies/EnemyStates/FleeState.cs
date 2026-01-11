using Unity.VisualScripting;
using UnityEngine;

public class FleeState : State
{
    protected Rigidbody2D rb;
    protected float speed;
    protected bool isStuck = false;
    protected float stuckTime;
    protected float stuckDuration = 1f;
    protected Vector2 lastWallPos;
    protected float lowSpeedTimer = 0f;
    protected Vector2 awayWallDir;
    Vector2 moveDir;
    

    public FleeState(Enemy enemy)
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

        if (enemy.target == null) return;

        Vector2 preferredDir;

        Collider2D wallColliders = Physics2D.OverlapCircle(enemy.transform.position, 2f, LayerMask.GetMask("Wall","Obstacle"));
        if (wallColliders)
        {
            Vector2 wallPos = wallColliders.ClosestPoint(enemy.transform.position);
            Vector2 toPlayer = (enemy.target.transform.position - enemy.transform.position).normalized;
            awayWallDir = ((Vector2)enemy.transform.position - wallPos).normalized;
            Vector2 goDir = new Vector2(-awayWallDir.y, awayWallDir.x);

            if (enemy.rb.linearVelocity.magnitude < 0.1f)
            {
                lowSpeedTimer += Time.fixedDeltaTime;
            }
            else
            {
                lowSpeedTimer = 0f;
            }

            if (lowSpeedTimer > 0.2f) // musí stát 0.2s, jinak žádný stuck
            {
                if (!isStuck)
                {
                    isStuck = true;
                    stuckTime = Time.time;
                }
                else
                {
                    goDir *= -1;
                }
            }
            else if (isStuck && Time.time > stuckTime + stuckDuration)
            {
                isStuck = false;
            }


            if (isStuck)
            {
                Vector2 relative = enemy.target.transform.position - enemy.transform.position;

                float xDir = Mathf.Sign(relative.x);
                float yDir = Mathf.Sign(relative.y);

                // vyber smìr podle toho, kde je hráè
                if (Mathf.Abs(relative.x) > Mathf.Abs(relative.y))
                {
                    // hráè je víc vlevo/vpravo než nahoøe/dole
                    preferredDir = new Vector2(xDir, 0f); // jen horizontální unstuck
                }
                else
                {
                    // hráè je víc nahoøe/dole
                    preferredDir = new Vector2(0f, yDir); // jen vertikální unstuck
                }
            }
            else
            {
                // normální pohyb podél zdi
                preferredDir = (Vector2.Dot(toPlayer, goDir) > 0) ? -goDir : goDir;
            }



            moveDir = preferredDir.normalized;

        }
        else
        {
            moveDir = (enemy.transform.position - enemy.target.transform.position).normalized;
        }

        if (awayWallDir == Vector2.zero)
            rb.linearVelocity = moveDir * speed;
        else
        {
            rb.linearVelocity = (moveDir*0.7f + awayWallDir*0.3f).normalized * speed;
        }

    
    }



    public override State ChangeState()
    {
        if (enemy.target == null)
        {
            return enemy.idleState;
        }
        if (Vector2.Distance(enemy.transform.position, enemy.target.transform.position) > enemy.chaseRange)
        {
            return enemy.attackState;
        }
        return this;
    }

    public override void FixedAI()
    {
        enemy.rb.linearVelocity = moveDir * enemy.speed;

        //enemy.rb.linearVelocity = Vector2.Lerp(enemy.rb.linearVelocity, moveDir * speed, 0.1f);
        //enemy.rb.position += enemy.rb.linearVelocity * Time.deltaTime;

    }

}
