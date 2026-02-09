using System.Collections.Generic;
using UnityEngine;

public class HallucinationEffect : IProjectileEffect
{
    private List<Projectile> linkedProjectiles; // all projectiles in this group
    private int spawnCount = 3; // number of projectiles to spawn
    private float spreadAngle = 15f; // degrees

    public IProjectileEffect Clone()
    {
        return new HallucinationEffect();
    }

    public void OnSpawn(Projectile p)
    {
        if (linkedProjectiles != null)
            return; // already spawned

        linkedProjectiles = new List<Projectile>();
        linkedProjectiles.Add(p); // include the original

        Vector2 baseDir = p.direction;

        for (int i = 1; i < spawnCount; i++)
        {
            GameObject cloneGO = PoolManager.Instance.Get(p.poolKey);
            Projectile cloneProj = cloneGO.GetComponentInChildren<Projectile>();

            // rotate direction for spread
            float angleOffset = ((i - (spawnCount - 1) / 2f) * spreadAngle);
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
                p.effects // pass same effect list
            );

            linkedProjectiles.Add(cloneProj);
        }
    }

    public void OnUpdate(Projectile p) { }

    public void OnHit(Projectile p, Enemy enemy) { }

    public void OnCollide(Projectile p, Collider2D collision) { }

    public void OnDestroy(Projectile p)
    {
        if (linkedProjectiles == null)
            return;

        // destroy all linked projectiles except this one
        foreach (var proj in linkedProjectiles)
        {
            if (proj != null && proj != p)
            {
                proj.DestroyProjectile();
            }
        }

        linkedProjectiles.Clear();
        linkedProjectiles = null;
    }

    public bool OnRangeExpired(Projectile p) => false; // don't block range logic
}
