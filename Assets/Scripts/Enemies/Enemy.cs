using System.Collections;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Health / Core Stats")]
    [HideInInspector]
    public int health;

    [HideInInspector]
    public float speed { get; protected set; }

    [HideInInspector]
    public int damage { get; protected set; }

    [HideInInspector]
    public float chaseRange { get; protected set; }

    [HideInInspector]
    public float attackDuration { get; protected set; }

    [HideInInspector]
    public float knockback;

    [HideInInspector]
    public float knockbackReduction = 0f;


    [Header("Attack Runtime")]
    [HideInInspector]
    public float attackCooldown;

    [HideInInspector]
    public float lastAttack;

    [HideInInspector]
    public bool isAttacking;

    [HideInInspector]
    public bool isCharging;

    [HideInInspector]
    public float chargeStart;

    [HideInInspector]
    public float chargeTime;


    [Header("Targeting")]
    [HideInInspector]
    public GameObject target;

    [HideInInspector]
    public Vector2 targetPosition;

    protected bool looksATarget = true;
    protected bool turns = true;


    [Header("Movement / Physics")]
    public Rigidbody2D rb { get; protected set; }


    [Header("State Machine")]
    [HideInInspector]
    public State idleState;

    [HideInInspector]
    public AttackState attackState;

    [HideInInspector]
    public ChaseState chaseState;

    [HideInInspector]
    public FleeState fleeState;

    protected State currentState;


    [Header("Projectiles / Pools")]
    protected string partPoolKey = "EnemyDeath";
    public string poolKey { get; protected set; }
    public string projKey { get; protected set; }


    [Header("Visuals / Animation")]
    public Animator animator;
    public Transform firepoint;

    [HideInInspector]
    public SpriteRenderer[] spriteRenderer;

    protected Color killParticleColor;


    [Header("Spawner / Systems")]
    [HideInInspector]
    EnemySpawner enemySpawner;


    protected virtual void Awake()
    {
        
    }

    public virtual void EnableEnemy()
    {
        SetPoolKeys();
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player");
        isAttacking = false;
        isCharging = false;
        InitializeStats();
        spriteRenderer = GetComponentsInChildren<SpriteRenderer>(true);

        animator = GetComponent<Animator>();
        animator.Rebind();
        animator.Update(0f);
        foreach (var p in animator.parameters)
        {
            switch (p.type)
            {
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(p.name, false);
                    break;
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(p.name, 0f);
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(p.name, 0);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    animator.ResetTrigger(p.name);
                    break;
            }
        }

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;


        currentState = idleState;
        
    }

    protected virtual void SetPoolKeys() { }

    private void TurnToTarget()
    {
        if (target != null && looksATarget)
        {
            targetPosition = target.transform.position;
            if (transform.position.x < targetPosition.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);

            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);

            }
        }
        else if (targetPosition != null && looksATarget)
        {
            if (transform.position.x < targetPosition.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);

            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);

            }
        }
        else if (!looksATarget && turns)
        {
            if (rb.linearVelocityX > 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    protected virtual void Update()
    {
        TurnToTarget();

        if (currentState != null)
        {
            currentState.AI();
            currentState = currentState.ChangeState();
        }

    }

    protected void FixedUpdate()
    {
        if (currentState != null)
        {
            currentState.FixedAI();
        }
    }

    protected IEnumerator HitEffect(bool didDamage)
    {
        foreach (SpriteRenderer s in spriteRenderer)
        {
            if (didDamage)
                s.color = Color.red;
            else
                s.color = Color.black;
        }

        yield return new WaitForSeconds(0.1f);

        foreach (SpriteRenderer s in spriteRenderer)
        {
            s.color = Color.white;
        }
    }


    public virtual void TakeDamage(int damage)
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

    public virtual void TakeHit(Player player,int damage,float knockback)
    {
        TakeDamage(damage);

        Vector2 knockbackDirection = (transform.position - player.transform.position).normalized;
        rb.AddForce(knockbackDirection * (knockback - knockbackReduction*knockback), ForceMode2D.Impulse);
    }

    protected abstract void InitializeStats();

    public virtual void KillEnemy()
    {
        ParticleSystem particles = PoolManager.Instance.Get(partPoolKey).GetComponent<ParticleSystem>();
        particles.transform.position = transform.position;
        var main = particles.main;
        main.startColor = killParticleColor;

        particles.Play();

        if (enemySpawner != null)
        {
            enemySpawner.RemoveEnemy(this);
        }

        StopAllCoroutines();

        PoolManager.Instance.Release(partPoolKey, particles.gameObject, 2f);

        PoolManager.Instance.Release(poolKey, gameObject);

    }

    private void OnEnable()
    {
        EnableEnemy();
    }

    public Enemy SpawnEnemy(EnemySpawner spawner,Vector2 position)
    {
        SetPoolKeys();
        GameObject enemyObj = PoolManager.Instance.Get(poolKey);
        enemyObj.transform.position = position;
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        enemy.enemySpawner = spawner;
        return enemy;
    }
}
