using UnityEngine;

public class ParkinsonProjectile : Projectile
{
    private Vector2 basePosition;
    private void Awake()
    {
        poolKey = "ParkinsonProjectile";
        partPoolKey = "ParkinsonProjectileParticle";
    }


    protected override void AI()
    {
        basePosition += direction * currentSpeed * Time.deltaTime;

        Vector2 perpendicular = new Vector2(-direction.y, direction.x);

        float shake = Mathf.Sin(Time.time * 40f) * 0.2f;

        parentObject.transform.position =
            basePosition + (Vector2)(perpendicular * shake);
    }

    public override void SetStats(Vector2 position, int damage, Vector2 direction, float speed, bool isHostile, float range, GameObject owner, float knockback,int passThrough, float size)
    {
        base.SetStats(position, damage, direction, speed, isHostile, range, owner, knockback,passThrough,size);

        basePosition = parentObject.transform.position;
        debuff = new ParkinsonDebuff(2, 45f);


    }
}
