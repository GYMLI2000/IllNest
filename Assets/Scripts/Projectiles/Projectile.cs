using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    protected int damage;
    protected Vector2 direction;
    protected float speed;
    public bool isHostile {protected set; get; }
    protected float range;
    protected Vector2 lastPos;
    protected float distance;
    protected float knockBack;
    protected int passThrough;

    protected string poolKey;
    protected string partPoolKey;

    protected GameObject parentObject;
    protected GameObject owner;
    protected Debuff debuff;

    [SerializeField]
    protected GameObject destroyParticlePrefab;
    private bool destroyed = false;


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

    public virtual void SetStats(Vector2 position,int damage, Vector2 direction, float speed, bool isHostile,float range,GameObject owner, float knockback, int passThrough)
    {
        this.damage = damage;
        this.direction = direction;
        this.speed = speed;
        this.isHostile = isHostile;
        this.range = range;
        this.distance = 0;
        this.owner = owner;
        this.knockBack = knockback;
        this.passThrough = passThrough;

        destroyed = false;
        lastPos = position;
    }


    protected abstract void AI();



    virtual protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isHostile)
        {
            var player = collision.gameObject.GetComponent<Player>();
            player.TakeHit(owner.GetComponent<Enemy>(),damage,knockBack);
            if ( debuff != null)
            {
                collision.gameObject.GetComponent<DebuffManager>().AddDebuff(debuff);
            }
            DestroyProjectile();
        }
        else if (collision.gameObject.CompareTag("Enemy") && !isHostile)
        {
            Enemy enemy;
            if ((enemy = collision.gameObject.GetComponent<Enemy>()) != null)
            {
                enemy.TakeHit(owner.GetComponent<Player>(),damage,knockBack);
            }
            DestroyProjectile();
        }
        else if (!collision.gameObject.CompareTag("Enemy") && !collision.gameObject.CompareTag("Player"))
        {
            if (passThrough > 0 && collision.gameObject.layer != LayerMask.NameToLayer("Wall"))
            {
                passThrough--;
            }
            else
            {
                DestroyProjectile();
            }
        }
    }

    protected void DestroyProjectile()
    {
        if (destroyed) return;
        else destroyed = true;

        ParticleSystem particles = PoolManager.Instance.Get(partPoolKey).GetComponent<ParticleSystem>();
        particles.transform.position = parentObject.transform.position;
        particles.Play();

        PoolManager.Instance.Release(partPoolKey, particles.gameObject, 2f);

        PoolManager.Instance.Release(poolKey, parentObject);
    }

}
