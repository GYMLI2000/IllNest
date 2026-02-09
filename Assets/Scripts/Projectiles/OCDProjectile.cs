using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class OCDProjectile : Projectile
{
    private float lastTurnTime;
    private float turnCooldown = 1f;
    public GameObject target;


    private void Awake()
    {
        poolKey = "OCDProjectile";
        partPoolKey = "OCDProjectileParticle";
        debuff = null;
        lastTurnTime = 0f;
    }

    protected override void AI()
    {
        Turn();
        parentObject.transform.Translate(direction * currentSpeed * Time.deltaTime, Space.World);

    }

    private void Turn()
    {
        if (Time.time - lastTurnTime < turnCooldown)
            return;

        if (target == null)
            return;

        Vector2 toPlayer = target.transform.position - parentObject.transform.position;

        if (Mathf.Abs(toPlayer.x) <= 0.1f)
        {
            direction = new Vector2(0, Mathf.Sign(toPlayer.y));
            lastTurnTime = Time.time;
        }

        else if (Mathf.Abs(toPlayer.y) <= 0.1f)
        {
            direction = new Vector2(Mathf.Sign(toPlayer.x), 0);
            lastTurnTime = Time.time;
        }
    }

    public override void SetStats(Vector2 position, int damage, Vector2 direction, float speed, bool isHostile, float range, GameObject owner, float knockback, int passThrough, float size)
    {
        base.SetStats(position, damage, direction, speed, isHostile, range, owner, knockback, passThrough, size);
        debuff = null;
        lastTurnTime = 0f;
    }
}
