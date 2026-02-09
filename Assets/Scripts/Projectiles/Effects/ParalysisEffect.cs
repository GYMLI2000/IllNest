using System.Collections.Generic;
using UnityEngine;

public class ParalysisEffect : IProjectileEffect
{
    private float duration;
    public bool OnRangeExpired(Projectile p)
    {
        p.currentSpeed = 0;
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            return false;
        }
        return true; 
    }

    public void OnCollide(Projectile p, Collider2D collision) { }
    public void OnDestroy(Projectile p) { 
    }
    public void OnHit(Projectile p, Enemy enemy) { }
    public void OnSpawn(Projectile p) {
        duration = 3f;
    }
    public void OnUpdate(Projectile p) { }

    public IProjectileEffect Clone()
    {
        return new ParalysisEffect();
    }
}
