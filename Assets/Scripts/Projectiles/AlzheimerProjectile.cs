using UnityEngine;

public class AlzheimerProjectile : Projectile
{
    private SpriteRenderer[] spriteRenderer;


    private void Awake()
    {
        poolKey = "AlzheimerProjectile";
        partPoolKey = "AlzheimerProjectileParticle";
        debuff = null;
        spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
    }

    protected override void AI()
    {
        foreach (SpriteRenderer s in spriteRenderer)
        {
            s.color = new Color(1f, 1f, 1f, Mathf.PingPong(Time.time, 1f));
        }
        parentObject.transform.Translate(direction * currentSpeed * Time.deltaTime, Space.World);

    }

    public override void SetStats(Vector2 position, int damage, Vector2 direction, float speed, bool isHostile, float range, GameObject owner, float knockback, int passThrough, float size)
    {
        base.SetStats(position, damage, direction, speed, isHostile, range, owner, knockback, passThrough, size);

        debuff = new AlzheimerDebuff(3, 3);


    }
}
