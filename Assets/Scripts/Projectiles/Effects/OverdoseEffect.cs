using UnityEngine;

public class OverdoseEffect : IProjectileEffect
{

    public void OnDestroy(Projectile p)
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject projectile = PoolManager.Instance.Get(p.poolKey);
            projectile.GetComponentInChildren<Projectile>().SetStats(
                p.transform.position
                , (int)(p.damage * 0.4f),
                Random.insideUnitCircle.normalized
                , p.speed
                , false
                , p.range/2
                , p.owner,
                p.knockBack,
                p.passThrough,
                p.size/2,
                null,
                false
                );

            projectile.transform.position = p.transform.position;
        }


    }

    public void OnCollide(Projectile p, Collider2D c) { }
    public void OnHit(Projectile p, Enemy enemy){}

    public void OnSpawn(Projectile p){}

    public void OnUpdate(Projectile p){}
    public bool OnRangeExpired(Projectile p) { return false; }

    public IProjectileEffect Clone()
    {
        return new OverdoseEffect();
    }
}
