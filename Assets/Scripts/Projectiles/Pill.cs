using UnityEngine;

public class Pill : Projectile
{
    private void Awake()
    {
        poolKey = "PillProjectile";
        partPoolKey = "PillHit";
    }

    protected override void AI()
    {
        parentObject.transform.Translate(direction * speed * Time.deltaTime, Space.World);
        transform.Rotate(0f,0f,360f * Time.deltaTime);
    }
}
