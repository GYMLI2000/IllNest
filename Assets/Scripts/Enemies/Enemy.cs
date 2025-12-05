using System.Collections;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected int health;
    public float speed { get; protected set; }
    public int damage { get; protected set; }
    public float chaseRange { get; protected set; }
    public float attackCooldown;
    public float lastAttack;
    public bool isAttacking;
    public bool isCharging;
    public float chargeStart;
    public float chargeTime;
    public float attackDuration { get; protected set; }
    public float knockback;
    public Animator animator;


    public GameObject target;
    public Vector2 targetPosition;

    public Rigidbody2D rb { get; protected set; }

    public State idleState;
    public AttackState attackState;

    protected string partPoolKey = "EnemyDeath";
    public string poolKey {get;protected set;}

    [SerializeField]
    protected GameObject projectilePrefab;

    [SerializeField]
    protected GameObject killParticlePrefab;
    protected Color killParticleColor;

    protected SpriteRenderer[] spriteRenderer;

    protected State currentState;

    protected virtual void Start()
    {
        InitializeStats();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();


        currentState = idleState;
        
    }


    protected void Update()
    {
        if (target != null)
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
        else if (targetPosition != null)
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

    protected abstract void InitializeStats();

    public virtual void KillEnemy()
    {
        ParticleSystem particles = PoolManager.Instance.Get(partPoolKey).GetComponent<ParticleSystem>();
        particles.transform.position = transform.position;
        var main = particles.main;
        main.startColor = killParticleColor;

        particles.Play();

        PoolManager.Instance.Release(partPoolKey, particles.gameObject, 2f);

        PoolManager.Instance.Release(poolKey, gameObject);
    }
}
