using UnityEngine;

public class SchizophreniaProjectile : Projectile
{
    private void Awake()
    {
        poolKey = "SchizophreniaProjectile";
        partPoolKey = "SchizophreniaProjectileParticle";
    }

    protected override void AI()
    {
        parentObject.transform.Translate(direction * currentSpeed * Time.deltaTime, Space.World);
        transform.Rotate(0f, 0f, 360f * Time.deltaTime);
        currentSpeed += 1.5f * Time.deltaTime;

    }


}
