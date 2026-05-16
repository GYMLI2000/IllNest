using System.Collections;
using UnityEngine;

public class SchizophreniaCloneAttackState : AttackState
{

    private new Enemy enemy;

    public SchizophreniaCloneAttackState(Enemy enemy) : base(enemy)
    {
        this.enemy = enemy;
    }

    public override void Attack()
    {


            Vector2 baseDir = (enemy.target.transform.position - enemy.transform.position).normalized;
            AudioManager.Instance.PlaySFX("SchizoAttack");

            for (int i = -1; i <= 1; i++)
            {

                Vector2 rotatedDir = Quaternion.Euler(0, 0, 30f * i) * baseDir;

                GameObject projObj = PoolManager.Instance.Get(enemy.projKey);
                var projectile = projObj.GetComponentInChildren<SchizophreniaProjectile>();

                projectile.SetStats(enemy.firepoint.position, 1, rotatedDir, 5f, true, 10, enemy.gameObject, enemy.knockback, 0, 3);


                projObj.transform.position = enemy.firepoint.position;
            }
        
    }

}
