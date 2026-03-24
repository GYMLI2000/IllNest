using UnityEngine;

public class SpitterAttackState : AttackState
{
    private new SpitterEnemy enemy;


    public SpitterAttackState(SpitterEnemy enemy) : base(enemy)
    {
        this.enemy = enemy;
    }


    public override void Attack()
    {
        AudioManager.Instance.PlaySFX("SpitterAttack");

        Vector2 baseDir = (enemy.target.transform.position - enemy.transform.position).normalized;

        GameObject projObj = PoolManager.Instance.Get(enemy.projKey);
        var projectile = projObj.GetComponentInChildren<SpitProjectile>();

        projectile.SetStats(enemy.firepoint.position, 1, baseDir, 5f, true, 10, enemy.gameObject, enemy.knockback, 0, 1);


        projObj.transform.position = enemy.firepoint.position;
    }

}
