using System.Collections;
using UnityEngine;

public class OCDAttackState : AttackState
{
    private new OCDEnemy enemy;



    public OCDAttackState(OCDEnemy enemy) : base(enemy)
    {
        this.enemy = enemy;
    }

    public override void Attack()
    {
        enemy.StartCoroutine(RowAttack());
    }
    private IEnumerator RowAttack()
    {
        int amount = enemy.hitpoints.Count;
        Vector2 baseDir;

        for (int b = 0; b < amount; b++)
        {
            Vector2 direction = enemy.target.transform.position - enemy.transform.position;



            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                baseDir = new Vector2(Mathf.Sign(direction.x), 0);
            else
                baseDir = new Vector2(0, Mathf.Sign(direction.y));

            GameObject projObj = PoolManager.Instance.Get(enemy.projKey);
            var projectile = projObj.GetComponentInChildren<OCDProjectile>();

            projectile.SetStats(enemy.firepoint.position, 1, baseDir, 5f, true, -1, enemy.gameObject, enemy.knockback);
            projectile.target = enemy.target;
            projObj.GetComponentInChildren<SpriteRenderer>().color = enemy.hitpoints.Find(hitpoint => hitpoint.sequenceIndex == b).hitpointColor;


            projObj.transform.position = enemy.firepoint.position;
            yield return new WaitForSeconds(0.2f);
        }
    }

    public override State ChangeState()
    {

        if (enemy.target == null && !enemy.isAttacking && !enemy.isCharging)
        {
            return enemy.idleState;
        }
        if (!enemy.isAttacking && !enemy.isCharging && (Mathf.Abs(enemy.target.transform.position.x - enemy.transform.position.x) > 0.1f && Mathf.Abs(enemy.target.transform.position.y - enemy.transform.position.y) > 0.1f))
        {
            return enemy.chaseState;
        }
        return this;
    }
}
