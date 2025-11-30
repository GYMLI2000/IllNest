using UnityEngine;

public class ADHDAttackState : AttackState
{
    private new ADHDEnemy enemy;

    public ADHDAttackState(ADHDEnemy enemy) : base(enemy)
    {
        this.enemy = enemy;
    }

    public override void FixedAI()
    {
        if (!enemy.isAttacking)
            return;

        enemy.rb.MovePosition(enemy.rb.position + enemy.dashDirection * 30 * Time.fixedDeltaTime);
        Debug.Log(enemy.dashDirection);
    }

    public override void AI()
    {
        base.AI();
    }

    public override void Attack()
    {
        if (enemy.isOriginal)
        {
            var cloneObj = PoolManager.Instance.Get(enemy.poolKey);
            cloneObj.transform.position = enemy.transform.position;
            var clone = cloneObj.GetComponent<ADHDEnemy>();
            clone.target = enemy.target;
            clone.isOriginal = false;
            enemy.clones.Add(clone);
        }
         enemy.dashDirection =
                (enemy.target.transform.position - enemy.transform.position).normalized;
    }


}
