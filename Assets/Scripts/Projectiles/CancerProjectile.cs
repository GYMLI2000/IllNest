using System.Collections.Generic;
using UnityEngine;

public class CancerProjectile : Projectile
{
    private void Awake()
    {
        poolKey = "CancerProjectile";
        partPoolKey = "CancerProjectileParticle";
        debuff = null;
    }

    protected override void AI()
    {
        parentObject.transform.Translate(direction * currentSpeed * Time.deltaTime, Space.World);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        float pulse = 2f + Mathf.Sin(Time.time * 2) * 0.5f;
        transform.localScale = Vector2.one * pulse;

    }

    public override void SetStats(Vector2 position, int damage, Vector2 direction, float speed, bool isHostile, float range, GameObject owner, float knockback, int passThrough, float size)
    {
        base.SetStats(position, damage, direction, speed, isHostile, range, owner, knockback, passThrough, size);

        debuff = new CancerDebuff(60, 2);


    }
}
