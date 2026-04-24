using System.Collections.Generic;
using UnityEngine;

public class CancerProjectile2 : Projectile
{
    private void Awake()
    {
        poolKey = "CancerProjectile2";
        partPoolKey = "CancerProjectileParticle";
        debuff = null;
    }

    [Header("Wave Settings")]
    public float amplitude = .05f;
    public float frequency = 1f;

    private Vector2 startPos;
    private float timeAlive;
    private Vector2 perpendicularDir;

    // Notice we removed the phaseOffset parameter here, returning to the base signature
    public override void SetStats(Vector2 position, int damage, Vector2 direction, float speed, bool isHostile, float range, GameObject owner, float knockback, int passThrough, float size)
    {
        base.SetStats(position, damage, direction, speed, isHostile, range, owner, knockback, passThrough, size);

        debuff = new CancerDebuff(60, 2);

        startPos = position;
        timeAlive = 0f;
        perpendicularDir = new Vector2(-direction.y, direction.x).normalized;

        parentObject.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
    }

    protected override void AI()
    {
        timeAlive += Time.deltaTime;

        Vector2 forwardMovement = direction * (currentSpeed * timeAlive);

        // Removed phase offset. It just uses timeAlive now!
        float waveOffset = Mathf.Sin(timeAlive * frequency) * amplitude;

        parentObject.transform.position = startPos + forwardMovement + (perpendicularDir * waveOffset);

        float pulse = 1f + Mathf.Sin(Time.time * 2) * 0.1f;
        transform.localScale = Vector2.one * pulse;
    }

}