using UnityEngine;

public class ParkinsonAttackState : AttackState
{

    public ParkinsonAttackState(Enemy enemy) : base(enemy)
    {
        this.enemy = enemy;
    }

    public override void Attack()
    {
        Vector2 baseDir = (enemy.target.transform.position - enemy.transform.position).normalized;

        for (int i = -1; i <= 1; i++)
        {

            Vector2 rotatedDir = Quaternion.Euler(0, 0, 45f * i) * baseDir;

            GameObject projObj = PoolManager.Instance.Get(enemy.projKey);
            var projectile = projObj.GetComponentInChildren<ParkinsonProjectile>();

            projectile.SetStats(enemy.firepoint.position, 1, rotatedDir, 5f, true, 10,enemy.gameObject,enemy.knockback,1);


            projObj.transform.position = enemy.firepoint.position;
        }
    }



}
