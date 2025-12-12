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

    protected string poolKey;
    protected string partPoolKey;

    protected GameObject parentObject;
    protected GameObject owner;
    protected Debuff debuff;

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

    public virtual void SetStats(Vector2 position,int damage, Vector2 direction, float speed, bool isHostile,float range,GameObject owner, float knockback)
    {
        this.damage = damage;
        this.direction = direction;
        this.speed = speed;
        this.isHostile = isHostile;
        this.range = range;
        this.distance = 0;
        this.owner = owner;
        this.knockBack = knockback;

        lastPos = position;
    }


    protected abstract void AI();



    virtual protected void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player") && isHostile)
        {
            var player = collision.gameObject.GetComponent<Player>();
            player.TakeHit(owner.GetComponent<Enemy>(),damage,knockBack);// damage to player
            if (!debuff.Equals(null))
            {
                collision.gameObject.GetComponent<DebuffManager>().AddDebuff(debuff);
            }
            

            DestroyProjectile();


        }
        else if (collision.gameObject.CompareTag("Enemy") && !isHostile)
        {

            var enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.TakeHit(owner.GetComponent<Player>(),damage,knockBack);// damage to enemy
            DestroyProjectile();


        }
        else if (!collision.gameObject.CompareTag("Enemy") && !collision.gameObject.CompareTag("Player"))
        {
            DestroyProjectile();
        }


    }

    protected void DestroyProjectile()
    {
        //ParticleSystem particles =Instantiate(destroyParticlePrefab, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        ParticleSystem particles = PoolManager.Instance.Get(partPoolKey).GetComponent<ParticleSystem>();
        particles.transform.position = parentObject.transform.position;
        particles.Play();

        //Destroy(particles.gameObject,2f);
        PoolManager.Instance.Release(partPoolKey, particles.gameObject, 2f);
        //Destroy(parentObject);
        PoolManager.Instance.Release(poolKey, parentObject);
    }

}
