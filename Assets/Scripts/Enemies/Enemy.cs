using System.Collections;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected int health;
    public float speed { get; protected set; }
    public int damage {get; protected set;}
    public float chaseRange { get; protected set; }

    public GameObject target;

    protected Rigidbody2D rb;



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
        currentState = new IdleState(this);
        Debug.Log("start");
    }





    protected void Update()
    {
        if (target != null)
        {
            if (transform.position.x < target.transform.position.x)
            {
                transform.rotation = new Quaternion(0, 180, 0, 0);
            }
            else
            {
                transform.rotation = Quaternion.identity;
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

    protected void KillEnemy()
    {
        ParticleSystem particles = Instantiate(killParticlePrefab, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();

        var main = particles.main;
        main.startColor = killParticleColor;

        particles.Play();

        Destroy(particles.gameObject, 2f);
        Destroy(gameObject);
    }
}
