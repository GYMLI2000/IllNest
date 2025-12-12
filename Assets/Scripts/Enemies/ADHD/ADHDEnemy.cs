using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ADHDEnemy : Enemy
{
    public Vector2 dashDirection;
    public List<Enemy> clones;
    public float dashPower {get;private set;}
    public List<Material> enemyMaterial;

    protected override void Start()
    {
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            enemyMaterial.Add(sr.material);
        }
        base.Start();
    }

    protected override void InitializeStats()
    {
        killParticleColor = new Color(50f / 255f, 227f / 255f, 50f /255f, 0.2f);
        chaseRange =  4; 
        damage = 1;
        speed = 7;
        health = 7;
        poolKey = "ADHDEnemy";
        isAttacking = false;
        attackCooldown = 2;
        attackDuration = .5f;
        chargeTime = 1.5f;
        lastAttack = Time.time;
        clones = new List<Enemy>();
        attackState = new ADHDAttackState(this);
        idleState = new IdleState(this);
        chaseState = new ChaseState(this);
        dashPower = 20;
        knockback = 2f;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isAttacking && (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Player")))
        {
            dashDirection = dashDirection/3 * -1;
        }
    }

    public override void KillEnemy()
    {

        foreach (Enemy clone in clones)
        {
            clone.KillEnemy();
        }

        base.KillEnemy();
    }

    public override void TakeDamage(int damage)
    {
        for (int i = clones.Count - 1; i >= 0; i--)
        {
            var clone = clones[i];

            if (!clone.gameObject.activeSelf)
                clones.RemoveAt(i);
        }

        if (clones.Count <= 0)
        {
            base.TakeDamage(damage);
        }
        else
        {
            StartCoroutine(HitEffect(false));
        }
    }
}
