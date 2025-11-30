using System.Collections;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected int health;
    public float speed { get; protected set; }
    public int damage { get; protected set; }
    public float chaseRange { get; protected set; }
    public float attackCooldown { get; protected set; }
    public float lastAttack;
    public bool isAttacking;
    public float attackDuration { get; protected set; }


    public GameObject target;

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
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentsInChildren<SpriteRenderer>();

        idleState = new IdleState(this);


        currentState = idleState;
        
    }





    protected void Update()
    {
        Debug.Log(currentState.GetType().ToString());
        if (target != null)
        {
            if (transform.position.x < target.transform.position.x)
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

    private IEnumerator HitEffect()
    {
        foreach (SpriteRenderer s in spriteRenderer)
        {
            s.color = Color.red;
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
            StartCoroutine(HitEffect());
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
