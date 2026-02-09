using UnityEngine;

public class ParkinsonProjectile : Projectile
{
    private void Awake()
    {
        poolKey = "ParkinsonProjectile";
        partPoolKey = "ParkinsonProjectileParticle";
    }

    protected override void AI()
    {
        parentObject.transform.Translate(direction * currentSpeed * Time.deltaTime, Space.World);

        Vector2 perpendicular = new Vector2(-direction.y, direction.x);


        float shake = Mathf.Sin(Time.time * 40) * .02f;


        parentObject.transform.Translate(perpendicular * shake, Space.World);
    }

    public override void SetStats(Vector2 position, int damage, Vector2 direction, float speed, bool isHostile, float range, GameObject owner, float knockback,int passThrough, float size)
    {
        base.SetStats(position, damage, direction, speed, isHostile, range, owner, knockback,passThrough,size);

        debuff = new ParkinsonDebuff(2, 45f);


    }
}
