using UnityEngine;

public class ReflexEffect : IProjectileEffect
{ 
    public void OnCollide(Projectile p, Collider2D collision)
    {
        p.bounceAmount--;

        Vector2 hitPoint = collision.ClosestPoint(p.transform.position);

        Vector2 normal = ((Vector2)p.transform.position - hitPoint).normalized;

        Vector2 reflected = Vector2.Reflect(p.direction.normalized, normal);

        p.SetDir(reflected, true);
    }
    public void OnSpawn(Projectile p) {
        p.bounceAmount = 3;
    }
    public void OnDestroy(Projectile p){}
    public void OnHit(Projectile p, Enemy enemy) { }
    public bool OnRangeExpired(Projectile p) { return false; }
    public void OnUpdate(Projectile p) { }

    public IProjectileEffect Clone()
    {
        return new ReflexEffect();
    }
}
