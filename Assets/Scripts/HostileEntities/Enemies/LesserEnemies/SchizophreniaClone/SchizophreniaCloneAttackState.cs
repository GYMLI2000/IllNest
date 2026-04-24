using System.Collections;
using UnityEngine;

public class SchizophreniaCloneAttackState : AttackState
{

    private new SchizophreniaClone enemy;

    public SchizophreniaCloneAttackState(SchizophreniaClone enemy) : base(enemy)
    {
        this.enemy = enemy;
    }

    public override void Attack()
    {
        if (UnityEngine.Random.value > 0.5f)
        {

            Vector2 baseDir = (enemy.target.transform.position - enemy.transform.position).normalized;
            AudioManager.Instance.PlaySFX("SchizoAttack");

            for (int i = -2; i <= 2; i++)
            {

                Vector2 rotatedDir = Quaternion.Euler(0, 0, 30f * i) * baseDir;

                GameObject projObj = PoolManager.Instance.Get(enemy.projKey);
                var projectile = projObj.GetComponentInChildren<SchizophreniaProjectile>();

                projectile.SetStats(enemy.firepoint.position, 1, rotatedDir, 5f, true, 10, enemy.gameObject, enemy.knockback, 0, 1);


                projObj.transform.position = enemy.firepoint.position;
            }
        }
        else
        {
            
            enemy.StartCoroutine(enemy.Dash());
        }
    }

    public override void FixedAI()
    {
        if (!enemy.isDashing)
            return;

        enemy.rb.linearVelocity = enemy.dashDirection* enemy.dashPower;
    }

}
