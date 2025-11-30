using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ADHDEnemy : Enemy
{
    public Vector2 dashDirection;
    public List<Enemy> clones;
    public bool isOriginal = true; // musim zmenit aby pri tahani z poolu byli vzdycky original dokud se nenastavi jinak

    protected override void Start()
    {
        InitializeStats();
        base.Start();
    }

    protected override void InitializeStats()
    {
        killParticleColor = new Color(148f / 255f, 43f / 255f, 148f /255f, 0.2f);
        chaseRange =  3; 
        damage = 1;
        speed = 7;
        health = 5;
        poolKey = "ADHDEnemy";
        isAttacking = false;
        attackCooldown = 3;
        attackDuration = .2f;
        lastAttack = Time.time;
        clones = new List<Enemy>();
        attackState = new ADHDAttackState(this);
    }

    public override void KillEnemy()
    {
        if (isOriginal)
        {
            foreach (Enemy clone in clones)
            {
                clone.KillEnemy();
            }
        }

        base.KillEnemy();
    }
}
