using UnityEngine;

public class SpitProjectile : Projectile
{
    private void Awake()
    {
        poolKey = "SpitProjectile";
        partPoolKey = "SpitProjectileParticle";
    }

    protected override void AI()
    {
        parentObject.transform.Translate(direction * currentSpeed * Time.deltaTime, Space.World);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

    }
}
