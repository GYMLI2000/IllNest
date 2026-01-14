using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AlzheimerAttackState : AttackState
{
    private new AlzheimerEnemy enemy;
    private bool isFading = false;
    private Vector2 moveDir;


    public AlzheimerAttackState(AlzheimerEnemy enemy) : base(enemy)
    {
        this.enemy = enemy;
    }

    public override void AI()
    {
        base.AI();

        if (enemy.rb.linearVelocity.magnitude > 0)
        {
            enemy.animator.SetBool("isWalking", true);
        }
        else
        {
            enemy.animator.SetBool("isWalking", false);
        }


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

        if (!isFading && enemy.lastTeleport + enemy.teleportCooldown <= Time.time)
        {
            enemy.lastTeleport = Time.time;
            enemy.StartCoroutine(FadeCoroutine());
        }

    }

    private void Shoot()
    { 
       var projObj = PoolManager.Instance.Get(enemy.projKey);
            var projectile = projObj.GetComponentInChildren<AlzheimerProjectile>();
        Vector2 shootDir = new Vector2(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)
                ).normalized;
            projectile.SetStats(enemy.firepoint.position, enemy.damage,shootDir,6,true,-1,enemy.gameObject,enemy.knockback,20);
            projObj.transform.position = enemy.firepoint.position;


    }



    private IEnumerator FadeCoroutine()
    {
        isFading = true;

        SpriteRenderer[] renderers = enemy.spriteRenderer;
        float alpha = 1f;

        // Fade out
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime;
            foreach (SpriteRenderer sr in renderers)
            {
                Color c = sr.color;
                c.a = alpha;
                sr.color = c;
            }
            yield return null;
        }

        alpha = 0f;
        foreach (SpriteRenderer sr in renderers)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }

        Teleport();

        // Fade in
        while (alpha < 1f)
        {
            alpha += Time.deltaTime;
            foreach (SpriteRenderer sr in renderers)
            {
                Color c = sr.color;
                c.a = alpha;
                sr.color = c;
            }
            yield return null;
        }


        foreach (SpriteRenderer sr in renderers)
        {
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;
        }

        isFading = false;
    }




    private void Teleport()
    {
        Vector2 origin = enemy.transform.position;
        Vector2 dir = Random.insideUnitCircle.normalized;
        float dist = Random.Range(5f, 10f);

        RaycastHit2D hit = Physics2D.CircleCast(origin, .5f, dir, dist, LayerMask.GetMask("Wall"));

        Vector2 targetPos;

        if (hit.collider != null)
        {

            targetPos = hit.point + hit.normal * 0.5f;
        }
        else
        {

            targetPos = origin + dir * dist;
        }

        enemy.transform.position = targetPos;
    }


    public override void Attack()
    {
        if (!isFading)
        {
            Shoot();
        }
    }

    public override State ChangeState()
    {
        if (!isFading && enemy.target == null)
        {
            return enemy.idleState;
        }
        return this;
    }

    public override void FixedAI()
    {
        enemy.rb.linearVelocity = Vector2.Lerp(enemy.rb.linearVelocity, moveDir * enemy.speed, 0.1f);
        enemy.rb.position += enemy.rb.linearVelocity * Time.deltaTime;
    }

    public void OnTriggerEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Collided with wall");

            moveDir = Vector2.Reflect(moveDir, collision.contacts[0].normal).normalized;

            enemy.rb.position += collision.contacts[0].normal * 0.1f;
        }
    }


}
