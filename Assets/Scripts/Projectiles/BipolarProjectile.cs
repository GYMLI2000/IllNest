using UnityEngine;

public class BipolarProjectile : Projectile
{
    private void Awake()
    {
        poolKey = "BipolarProjectile";
        partPoolKey = "BipolarProjectileParticle";
        debuff = null;
    }

    protected override void AI()
    {
        parentObject.transform.Translate(direction * speed * Time.deltaTime, Space.World);

        float pulse = 2f + Mathf.Sin(Time.time * 2) * 0.5f;
        transform.localScale = Vector2.one * pulse;
        transform.Rotate(0f, 0f, 90f * Time.deltaTime);

    }

    public override void SetStats(Vector2 position, int damage, Vector2 direction, float speed, bool isHostile, float range, GameObject owner, float knockback, int passThrough)
    {
        base.SetStats(position, damage, direction, speed, isHostile, range, owner, knockback, passThrough);

        debuff = new DeppresionDebuff(2, 2);


    }
}
