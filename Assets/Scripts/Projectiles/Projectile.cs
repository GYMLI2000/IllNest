using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using Random = UnityEngine.Random;

public abstract class Projectile : MonoBehaviour
{
    public int damage { protected set; get; }
    public Vector2 direction { protected set; get; }
    public float speed { protected set; get; }
    public float currentSpeed;
    public bool isHostile {protected set; get; }
    public float range { protected set; get; }
    protected Vector2 lastPos;
    protected float distance;
    public float knockBack { protected set; get; }
    public float size { protected set; get; }
    public int passThrough { protected set; get; }
    public int bounceAmount;

    public string poolKey { protected set; get; }
    protected string partPoolKey;

    protected GameObject parentObject;
    public GameObject owner { protected set; get; }
    protected Debuff debuff;
    public List<IProjectileEffect> effects = null;

    [SerializeField]
    protected GameObject destroyParticlePrefab;
    private bool destroyed = false;

    public static event Action<bool> ProjectileHit;

    private void Awake()
    {
        parentObject = transform.parent.gameObject;
    }

    protected void Update()
    { 
        AI();
        CheckRange();

        if (effects != null && effects.Count > 0)
        {
            foreach (var effect in effects)
            {
                effect.OnUpdate(this);
            }
        }
    }

    protected void CheckRange()
    {
        if (range == -1) return; // inf range


        distance += Vector2.Distance(lastPos, (Vector2)transform.position);
        lastPos = transform.position;

        if (distance > range)
        {
            bool handled = false;

            if (effects != null)
            {
                foreach (var effect in effects)
                {
                    if (effect.OnRangeExpired(this))
                    {
                        handled = true;
                        break;
                    }
                }
            }

            if (!handled)
                DestroyProjectile();
        }
    }

    public virtual void SetStats(Vector2 position,int damage, Vector2 direction, float speed, bool isHostile,float range,GameObject owner, float knockback, int passThrough,float size)
    {
        this.damage = damage;
        this.direction = direction;
        this.speed = speed;
        this.currentSpeed = speed;
        this.isHostile = isHostile;
        this.range = range;
        this.distance = 0;
        this.owner = owner;
        this.knockBack = knockback;
        this.passThrough = passThrough;
        this.size = size;

        destroyed = false;
        lastPos = position;
        parentObject = transform.parent.gameObject;
        parentObject.transform.localScale = size * Vector3.one;
        this.effects = null;

    }

    public virtual void SetStats(Vector2 position, int damage, Vector2 direction, float speed, bool isHostile, float range, GameObject owner, float knockback, int passThrough, float size, List<IProjectileEffect> effects)
    {
        SetStats(position,damage, direction, speed, isHostile, range, owner, knockback, passThrough, size);

        if (effects != null && effects.Count > 0)
        {
            this.effects = new List<IProjectileEffect>(effects.Count);

            foreach (var effect in effects)
            {
                this.effects.Add(effect.Clone());
            }
        }
        else
        {
            this.effects = null;
        }

        if (this.effects != null && this.effects.Count > 0)
        {
            foreach (var effect in this.effects)
            {
                effect.OnSpawn(this);
            }
        }
    }


    protected abstract void AI();



    virtual protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isHostile)
        {
            var player = collision.gameObject.GetComponent<Player>();
            player.TakeHit(owner.GetComponent<Enemy>(),damage,knockBack);
            if ( debuff != null && Random.Range(0f, 1f) <= 0.5f - player.diseaseImunity*0.1f)
            {
                collision.gameObject.GetComponent<DebuffManager>().AddDebuff(debuff);
                ProjectileHit?.Invoke(true);
            }
            DestroyProjectile();
        }
        else if (collision.gameObject.CompareTag("Enemy") && !isHostile)
        {
            Enemy enemy;
            if ((enemy = collision.gameObject.GetComponent<Enemy>()) != null)
            {
                enemy.TakeHit(owner.GetComponent<Player>(),damage,knockBack);

                if (effects != null && effects.Count > 0)
                {
                    foreach (var effect in effects)
                    {
                        effect.OnHit(this,enemy);
                    }
                }
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
                if (!isHostile)
                {
                    ProjectileHit?.Invoke(false);
                    if (effects != null && effects.Count > 0)
                    {
                        foreach (var effect in effects)
                        {
                            effect.OnCollide(this,collision);
                        }
                    }
                }
                if (bounceAmount <= 0)
                {
                    DestroyProjectile();
                }
            }
        }
    }

    public void SetDir(Vector2 dir, bool resetRange)
    {
        direction = dir;
        if (resetRange) { distance = 0; }
    }

    public void DestroyProjectile()
    {
        if (destroyed) return;
        else destroyed = true;

        if (effects != null && effects.Count > 0)
        {
            foreach (var effect in effects)
            {
                effect.OnDestroy(this);
            }
        }

        ParticleSystem particles = PoolManager.Instance.Get(partPoolKey).GetComponent<ParticleSystem>();
        particles.transform.position = parentObject.transform.position;
        particles.Play();

        PoolManager.Instance.Release(partPoolKey, particles.gameObject, 2f);

        PoolManager.Instance.Release(poolKey, parentObject);
    }

}
