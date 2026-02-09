using UnityEngine;

public interface IProjectileEffect
{
    IProjectileEffect Clone();

    void OnSpawn(Projectile p);
    void OnUpdate(Projectile p);
    void OnHit(Projectile p, Enemy enemy);
    void OnDestroy(Projectile p);
    void OnCollide(Projectile p, Collider2D collision);
    bool OnRangeExpired(Projectile p);
}
