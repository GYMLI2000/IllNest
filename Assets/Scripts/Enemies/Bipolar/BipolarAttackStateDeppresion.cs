using System.Collections;
using UnityEngine;

public class BipolarAttackStateDeppresion : AttackState
{
    private new BipolarEnemy enemy;


    public BipolarAttackStateDeppresion(BipolarEnemy enemy) : base(enemy)
    {
        this.enemy = enemy;
    }

    public override void Attack()
    {
        enemy.StartCoroutine(BurstAttack());
    }

    private IEnumerator BurstAttack()
    {
        int burstCount = 5;
        Vector2 baseDir = (enemy.target.transform.position - enemy.transform.position).normalized;

        for (int b = 0; b < burstCount; b++)
        {
            Vector2 rotatedDir = Quaternion.Euler(0, 0, Random.Range(-45f, 45f)) * baseDir;

            GameObject projObj = PoolManager.Instance.Get(enemy.projKey);
            var projectile = projObj.GetComponentInChildren<BipolarProjectile>();

            projectile.SetStats(enemy.firepoint.position, 1, rotatedDir, 2f, true, -1, enemy.gameObject, enemy.knockback,0,1);


            projObj.transform.position = enemy.firepoint.position;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public override State ChangeState()
    {
        if (enemy.target == null && !enemy.isAttacking && !enemy.isCharging)
        {
            return enemy.idleState;
        }
        if (!enemy.isAttacking && !enemy.isCharging && enemy.isEnraged)
        {
            return enemy.chaseState;
        }
        return this;
    }
}
