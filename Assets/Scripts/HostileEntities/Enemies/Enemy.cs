using System;
using System.Collections;
using UnityEngine;

public abstract class Enemy : HostileEntity
{
    protected bool isLesser = false;
    protected string deathAudio;
    protected bool isDead = false;

    [Header("Attack Runtime")]
    [HideInInspector] public float attackCooldown;
    [HideInInspector] public float lastAttack;
    [HideInInspector] public bool isAttacking;
    [HideInInspector] public float chaseRange;
    [HideInInspector] public float attackDuration;
    [HideInInspector] public bool isCharging;
    [HideInInspector] public float chargeStart;
    [HideInInspector] public float chargeTime;

    [Header("State Machine")]
    [HideInInspector] public State idleState;
    [HideInInspector] public AttackState attackState;
    [HideInInspector] public ChaseState chaseState;
    [HideInInspector] public FleeState fleeState;
    protected State currentState;


    [Header("Spawner / Systems")]
    [HideInInspector] public EnemySpawner enemySpawner;

    private void OnEnable()
    {
        EnableEnemy();
    }

    public virtual void EnableEnemy()
    {
        isDead = false;
        base.EnableEntity();

        isAttacking = false;
        isCharging = false;
        currentState = idleState;
    }


    protected virtual void Update()
    {
        TurnToTarget();

        if (currentState != null)
        {
            currentState.AI();
            currentState = currentState.ChangeState();
        }
    }

    protected virtual void FixedUpdate()
    {
        if (currentState != null)
        {
            currentState.FixedAI();
        }
    }

    public override void Die()
    {
        if (isDead) return; // Prevent multiple death triggers
        base.Die();
        isDead = true;
        ParticleSystem particles = PoolManager.Instance.Get(partPoolKey).GetComponent<ParticleSystem>();
        particles.transform.position = transform.position;
        var main = particles.main;
        main.startColor = killParticleColor;
        particles.Play();

        if (deathAudio != null)
            AudioManager.Instance.PlaySFX(deathAudio);
        else if (!isLesser)
            AudioManager.Instance.PlaySFX("EnemyDeath1");
        else
            AudioManager.Instance.PlaySFX("LesserEnemyDeath1");

        if (enemySpawner != null)
        {
            enemySpawner.RemoveEnemy(this);
        }

        StopAllCoroutines();

        if (!isLesser)
        {
            CompletionManager.Instance.CheckCompletion("3501", 1, 10);
        }

        PoolManager.Instance.Release(partPoolKey, particles.gameObject, 2f);
        PoolManager.Instance.Release(poolKey, gameObject);
    }

    public override void Die(bool isByPlayer)
    {
        if (isDead) return; // Prevent multiple death triggers

        base.Die(isByPlayer);
        isDead = true;
        ParticleSystem particles = PoolManager.Instance.Get(partPoolKey).GetComponent<ParticleSystem>();
        particles.transform.position = transform.position;
        var main = particles.main;
        main.startColor = killParticleColor;
        particles.Play();

        if (!isLesser)
            AudioManager.Instance.PlaySFX("EnemyDeath1");
        else
            AudioManager.Instance.PlaySFX("LesserEnemyDeath1");

        if (enemySpawner != null)
        {
            enemySpawner.RemoveEnemy(this);
        }

        StopAllCoroutines();

        PoolManager.Instance.Release(partPoolKey, particles.gameObject, 2f);
        PoolManager.Instance.Release(poolKey, gameObject);
    }


    public Enemy SpawnEnemy(EnemySpawner spawner, Vector2 position)
    {
        SetPoolKeys();
        GameObject enemyObj = PoolManager.Instance.Get(poolKey);

        if (!enemyObj.activeSelf)
        {
            Debug.LogWarning($"Object from pool '{poolKey}' was not active! Forcing activation.");
            enemyObj.SetActive(true);
        }

        enemyObj.transform.position = position;
        Enemy enemy = enemyObj.GetComponent<Enemy>();

        if (enemy == null)
        {
            Debug.LogError("Spawned object missing Enemy component!");
            return null;
        }

        enemy.enemySpawner = spawner;
        return enemy;
    }
}
