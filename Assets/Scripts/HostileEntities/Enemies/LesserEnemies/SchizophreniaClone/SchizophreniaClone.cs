using System.Collections;
using UnityEngine;

public class SchizophreniaClone : Enemy
{
    private float realness = 0f;
    private float currentRealness = 0f;
    public SchizophreniaBoss boss;



    protected override void InitializeStats()
    {
        killParticleColor = new Color(120f/255f, 152f/255f, 209f/255f, 0.2f);
        chaseRange =  5;
        damage = 1;
        speed = 3;
        health = 1;
        idleState = new IdleState(this);
        chaseState = new ChaseState(this);
        attackState = new SchizophreniaCloneAttackState(this);
        attackDuration = 1f;
        chargeTime = 1.5f;
        realness = 0f;
        currentRealness = -1f;
        attackCooldown = 3f;

        knockback = 2f;
        isLesser = true;

    }

    protected override void Update()
    {
        base.Update();

        if ( realness != currentRealness)
        {
            currentRealness = realness;
            foreach (var sr in spriteRenderers)
            {
                Color c = sr.color;
                c.a = 0.5f + 0.5f * realness; 
                sr.color = c;
            }
        }

        if ( !isAttacking && !isCharging && currentRealness >= 1)
        {
            StartCoroutine(Clone());
            realness = 0f;
        }
    }

    private IEnumerator Clone()
    {
        isAttacking = true;
        isCharging = true;
        animator.SetTrigger("Clone");
        yield return new WaitForSeconds(0.8f);

        PoolManager.Instance.Get("SchizophreniaClone", 0.05f, minionObj =>
        {
            minionObj.transform.position = transform.position + ((target.transform.position.x - transform.position.x) > 0 ? Vector3.right : Vector3.left);
            var clone = minionObj.GetComponent<SchizophreniaClone>();
            clone.EnableEnemy();
            //boss.AddClone(clone);

        });

        realness = 0f;

        yield return new WaitForSeconds(2);

        isAttacking = false;
        isCharging = false;
    }

    public override void TakeHit(Player player, int damageAmount, float knockbackAmount)
    {
        realness += 0.2f;
    }

    public void Kill()
    {
        base.Die();
    }

    protected override void SetPoolKeys()
    {
        poolKey = "SchizophreniaClone";
        projKey = "SchizophreniaProjectile";
    }
}
