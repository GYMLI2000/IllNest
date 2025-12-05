using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

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

        enemy.rb.linearVelocity = enemy.dashDirection* enemy.dashPower;
    }

    public override void AI()
    {
        for (int i = enemy.clones.Count - 1; i >= 0; i--)
        {
            var clone = enemy.clones[i];

            if (!clone.gameObject.activeSelf)
                enemy.clones.RemoveAt(i);
        }

        ChangeCloneState();

        base.AI();


        if (!enemy.isAttacking)
        {
            enemy.knockback = 10f;
        }
        else
        {
            enemy.knockback = 50f;
        }

    }

    private void ChangeCloneState()
    {
        int count = enemy.clones.Count;
        foreach (var material in enemy.enemyMaterial)
        {
            material.SetFloat("_Grayscale", Mathf.Clamp01(count > 0? 1: 0));
        }
            
    }

    public override void Attack()
    {



        if (enemy.clones.Count < 3)
        {
            var pos = enemy.transform.position;
            PoolManager.Instance.Get("ADHDDistraction",0.05f, cloneObj =>
            {
                cloneObj.transform.position = pos;
                var clone = cloneObj.GetComponent<ADHDDistraction>();
                enemy.clones.Add(clone);
            }
            
            );
        }
         enemy.dashDirection =
                (enemy.target.transform.position - enemy.transform.position).normalized;
    }


}
