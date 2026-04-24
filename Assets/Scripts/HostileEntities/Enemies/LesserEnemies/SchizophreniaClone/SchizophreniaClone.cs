using System.Collections;
using UnityEngine;

public class SchizophreniaClone : Enemy
{
    private float realness = 0f;
    private float currentRealness = 0f;
    public SchizophreniaBoss boss;

    public float dashPower;
    public bool isDashing = false;
    public Vector2 dashDirection;


    protected override void InitializeStats()
    {
        killParticleColor = new Color(97f/255f, 128f/255f, 80f/255f, 0.2f);
        chaseRange =  5;
        damage = 1;
        speed = 8;
        health = 1;
        idleState = new IdleState(this);
        chaseState = new ChaseState(this);
        attackDuration = 1f;
        chargeTime = 1.5f;
        dashPower = 10;

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
        }
    }

    private IEnumerator Clone()
    {
        isAttacking = true;
        isCharging = true;

        PoolManager.Instance.Get("SchizophreniaClone", 0.05f, minionObj =>
        {
            minionObj.transform.position = transform.position;
            var clone = minionObj.GetComponent<SchizophreniaClone>();
            clone.EnableEnemy();
            boss.AddClone(clone);
        });

        realness = 0f;

        yield return new WaitForSeconds(3);

        isAttacking = false;
        isCharging = false;
    }

    public IEnumerator Dash()
    {
        isDashing = true;
        yield return new WaitForSeconds(1f);
        isDashing = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Projectile") && !collision.gameObject.GetComponent<Projectile>().isHostile && collision.gameObject.GetComponent<Projectile>().isOriginal)
        {
            realness += 0.2f;
        }
    }

    protected override void SetPoolKeys()
    {
        poolKey = "SchizophreniaClone";
        projKey = "SchizophreniaProjectile";
    }
}
