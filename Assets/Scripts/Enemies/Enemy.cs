using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField]
    protected int health;
    [SerializeField]
    protected float speed;
    [SerializeField]
    protected float damage;

    protected Rigidbody2D rb;

    [SerializeField]
    protected GameObject target;

    [SerializeField] 
    protected GameObject projectilePrefab;

    protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    abstract protected void AI();

    abstract protected void FixedAI();

    

    protected void Update()
    {
        if (transform.position.x < target.transform.position.x)
        {
            transform.rotation = new Quaternion(0, 180, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }

        AI();

    }

    protected void FixedUpdate()
    {
        FixedAI();
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;

        if (health < 0)
        {
            Destroy(gameObject);
        }
    }
}
