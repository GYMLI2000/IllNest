using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    protected int damage;
    protected Vector2 direction;
    protected float speed;
    protected bool isHostile;
    protected float range;
    protected Vector2 lastPos;
    protected float distance;
    protected float knockBack;

    protected GameObject parentObject;

    [SerializeField]
    protected GameObject destroyParticlePrefab;


    private void Start()
    {
        parentObject = transform.parent.gameObject;
    }

    protected void Update()
    { 
        AI();
        CheckRange();
    }

    protected void CheckRange()
    {
        if (range == -1) return; // inf range


        distance += Vector2.Distance(lastPos, (Vector2)transform.position);
        lastPos = transform.position;

        if(distance > range) DestroyProjectile();
    }

    public void SetStats(int damage, Vector2 direction, float speed, bool isHostile,float range)
    {
        this.damage = damage;
        this.direction = direction;
        this.speed = speed;
        this.isHostile = isHostile;
        this.range = range;

        lastPos = transform.position;
    }

    protected abstract void AI();



    virtual protected void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject);

        if (collision.gameObject.CompareTag("Player") && isHostile)
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(damage); // damage to player
            DestroyProjectile();


        }
        else if (collision.gameObject.CompareTag("Enemy") && !isHostile)
        {

            collision.gameObject.GetComponent<Enemy>().TakeDamage(damage); // damage to enemy
            DestroyProjectile();


        }
        else if (!collision.gameObject.CompareTag("Enemy") && !collision.gameObject.CompareTag("Player"))
        {
            DestroyProjectile();
        }


    }

    protected void DestroyProjectile()
    {
        ParticleSystem particles =Instantiate(destroyParticlePrefab, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        particles.Play();

        Destroy(particles.gameObject,2f);
        Destroy(parentObject);
    }

}
