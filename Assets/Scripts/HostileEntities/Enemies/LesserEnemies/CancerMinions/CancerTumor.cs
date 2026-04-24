using System.Collections;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class CancerTumor : Enemy
{
    protected override void InitializeStats()
    {
        killParticleColor = new Color(97f/255f, 128f/255f, 80f/255f, 0.2f);
        chaseRange =  100000;
        damage = 1;
        speed = 0;
        health = 10;
        idleState = new IdleState(this);
        knockback = 2f;
        isLesser = true;
        knockbackReduction = 1f;
        turns = false;
        looksATarget = false;

    }

    protected override void SetPoolKeys()
    {
        poolKey = "CancerTumor";
    }

    public override void EnableEnemy()
    {
        base.EnableEnemy();
        StartCoroutine(SummonTumor());
    }

    private IEnumerator SummonTumor()
    {
        animator.SetTrigger("Summon");
        damage = 0;
        speed = 0;
        yield return new WaitForSeconds(1.5f);

        damage = 1;
    }
}
