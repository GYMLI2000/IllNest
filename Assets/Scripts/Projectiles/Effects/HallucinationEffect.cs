using System.Collections.Generic;
using UnityEngine;

public class HallucinationEffect : IProjectileEffect
{
    private List<Projectile> linkedProjectiles; 
    private int spawnCount = 3; 
    private float spreadAngle = 30f; 

    public IProjectileEffect Clone()
    {
        return new HallucinationEffect();
    }

    public void OnSpawn(Projectile p)
    {
        if (linkedProjectiles != null || p.isOriginal == false)
            return; 

        linkedProjectiles = new List<Projectile>();
        linkedProjectiles.Add(p); 

        Vector2 baseDir = p.direction;

        for (int i = -1; i < spawnCount; i+=2)
        {
            GameObject cloneGO = PoolManager.Instance.Get(p.poolKey);
            Projectile cloneProj = cloneGO.GetComponentInChildren<Projectile>();


            float angleOffset = (i * spreadAngle);
            Vector2 dir = Quaternion.Euler(0, 0, angleOffset) * baseDir;

            cloneProj.SetStats(
                (Vector2)p.transform.position,
                p.damage,
                dir,
                p.speed,
                p.isHostile,
                p.range,
                p.owner,
                p.knockBack,
                p.passThrough,
                p.size,
                p.effects,
                false
            );

            cloneProj.isOriginal = false;
            linkedProjectiles.Add(cloneProj);
        }

    }

    public void OnUpdate(Projectile p)
    {
        if (linkedProjectiles == null) return;

        linkedProjectiles.RemoveAll(proj => proj == null || !proj.isActiveAndEnabled);
    }

    public void OnHit(Projectile p, Enemy enemy) { }

    public void OnCollide(Projectile p, Collider2D collision) { }

    public void OnDestroy(Projectile p)
    {
        if (linkedProjectiles == null || !p.isOriginal)
            return;


        foreach (var proj in linkedProjectiles)
        {
            if (proj != null && proj != p && proj.isActiveAndEnabled)
            {
                proj.DestroyProjectile();
            }
        }

        linkedProjectiles.Clear();
        linkedProjectiles = null;
    }

    public bool OnRangeExpired(Projectile p) { return false; }
}
