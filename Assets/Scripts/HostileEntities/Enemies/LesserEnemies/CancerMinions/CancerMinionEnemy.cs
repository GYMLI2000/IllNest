using System.Collections;
using UnityEngine;

public class CancerMinionEnemy : Enemy
{


    protected override void InitializeStats()
    {
        killParticleColor = new Color(97f/255f, 128f/255f, 80f/255f, 0.2f);
        chaseRange =  -1;
        damage = 1;
        speed = 3;
        health = 15;
        idleState = new IdleState(this);
        chaseState = new ChaseState(this);
        knockback = 2f;
        isLesser = true;
        knockbackReduction = 0.5f;

    }

    protected override void SetPoolKeys()
    {
        poolKey = "CancerMinion";
    }

    public override void EnableEnemy()
    {
        base.EnableEnemy();
        StartCoroutine(SummonMinion());
    }

    private IEnumerator SummonMinion()
    {
        animator.SetTrigger("Summon");
        damage = 0;
        speed = 0;
        yield return new WaitForSeconds(1.5f);

        damage = 1;
        speed = 3;
    }
}
