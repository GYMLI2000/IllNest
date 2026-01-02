using UnityEngine;

public class BipolarEnemy : Enemy
{
    public bool isEnraged { get; protected set; }
    protected float switchCooldown = 10f;
    protected float lastSwitchTime;
    public AttackState attackStateManic;

    [SerializeField]
    private Collider2D deppresionCollider;
    [SerializeField]
    private Collider2D manicCollider;

    [SerializeField]
    protected GameObject deppresionState;
    [SerializeField]
    protected GameObject manicState;

    protected override void SetPoolKeys()
    {
        poolKey = "BipolarEnemy";
        projKey = "BipolarProjectile";

    }

    protected override void InitializeStats()
    {
        killParticleColor = new Color( 0, 50 / 255f, 200f /255f, 0.2f);
        chaseRange =  3;
        damage = 1;
        speed = 5;
        health = 10;
        isAttacking = false;
        attackCooldown = 2;
        attackDuration = 0.5f;
        chargeTime = 0.6f;
        lastAttack = Time.time;
        attackState = new BipolarAttackStateDeppresion(this);
        attackStateManic = new BipolarAttackStateManic(this);
        idleState = new BipolarIdleState(this);
        chaseState = new BipolarChaseState(this);
        knockback = 1f;
        isEnraged = false;
        lastSwitchTime = Time.time;
    }

    protected override void Update()
    {


        if (isEnraged && Time.time > lastSwitchTime + switchCooldown && !isAttacking && !isCharging) //calm state
        {
            isEnraged = false;
            deppresionState.SetActive(true);
            deppresionCollider.enabled = true;
            manicState.SetActive(false);
            manicCollider.enabled = false;
            lastSwitchTime = Time.time;

            damage /= 2;
            attackDuration = 1f;
            knockback = 1f;

        }
        else if (!isEnraged && Time.time > lastSwitchTime + switchCooldown && !isAttacking && !isCharging) //enraged state
        {
            isEnraged = true;
            deppresionState.SetActive(false);
            deppresionCollider.enabled = false;
            manicState.SetActive(true);
            manicCollider.enabled = true;
            lastSwitchTime = Time.time;

            damage *= 2;
            attackDuration = 0.5f;
            knockback = 10f;

        }

        base.Update();
    }

    public override void TakeDamage(int damage)
    {
        if (isEnraged)
        {
            StartCoroutine(HitEffect(false));
        }
        else
        {
            health -= damage;

            if (health <= 0)
            {
                KillEnemy();
            }
            else
            {
                StartCoroutine(HitEffect(true));
            }
        }
    }
}
