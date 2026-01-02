using UnityEngine;

public class AlzheimerIdleState : IdleState
{
    Vector2 moveDir;
    new AlzheimerEnemy enemy;

    public AlzheimerIdleState(AlzheimerEnemy enemy) : base(enemy)
    {
        this.enemy = enemy;
    }

    public override void AI()
    {

        if (enemy.lastMove + enemy.moveCooldown <= Time.time)
        {
            enemy.lastMove = Time.time;

            moveDir = new Vector2(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            ).normalized;

            enemy.targetPosition = new Vector2(moveDir.x + enemy.transform.position.x, moveDir.y + enemy.transform.position.y);
        }

        Collider2D wallCollider = Physics2D.OverlapCircle(
        enemy.transform.position,
        1f,
        LayerMask.GetMask("Wall")
        );

        if (wallCollider != null)
        {
            Vector2 toWall = wallCollider.ClosestPoint(enemy.transform.position)
                             - (Vector2)enemy.transform.position;


            if (Vector2.Dot(moveDir, toWall.normalized) > 0f)
            {

                moveDir = (-toWall).normalized;
            }
        }
    }

    public override State ChangeState()
    {
        if (enemy.target != null)
        {
            return enemy.attackState;
        }
        else
        {
            return this;
        }
    }

    public override void FixedAI()
    {
        enemy.rb.linearVelocity = Vector2.Lerp(enemy.rb.linearVelocity, moveDir * enemy.speed, 0.1f);
        enemy.rb.position += enemy.rb.linearVelocity * Time.deltaTime;
    }

    public void On(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            moveDir = -moveDir;
        }
    }

}
