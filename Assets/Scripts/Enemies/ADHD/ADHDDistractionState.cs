using UnityEngine;

public class ADHDDistractionState : IdleState
{
    Vector2 moveDir;

    public ADHDDistractionState(Enemy enemy) : base(enemy)
    {
    }

    public override State ChangeState()
    {
        return this;
    }

    public override void AI()
    {

        if (enemy.lastAttack + enemy.attackCooldown <= Time.time)
        {
            enemy.lastAttack = Time.time;

            moveDir = new Vector2(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            ).normalized;

            enemy.targetPosition = new Vector2(moveDir.x + enemy.transform.position.x, moveDir.y + enemy.transform.position.y);
            enemy.attackCooldown = Random.Range(0.2f, 0.6f);
        }
    }

    public override void FixedAI()
    {
        //enemy.rb.linearVelocity =  moveDir * enemy.speed;

        enemy.rb.linearVelocity = Vector2.Lerp(enemy.rb.linearVelocity, moveDir * enemy.speed, 0.1f);
        enemy.rb.position += enemy.rb.linearVelocity * Time.deltaTime;
    }
}
