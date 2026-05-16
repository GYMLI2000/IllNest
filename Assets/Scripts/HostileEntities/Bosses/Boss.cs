using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

public abstract class Boss : HostileEntity
{
    [Header("Boss - HP")]
    [SerializeField] protected int maxHP = 500;

    [Header("Boss - Phases")]
    protected int currentPhase = 0;
    protected bool phaseChanging = false;

    [Header("Boss - Pattern")]
    protected Coroutine patternRoutine;
    protected bool isInPattern = false;

    [SerializeReference] protected Sprite icon;
    public static event Action<Color,Sprite,int,int> ActivateBossBar;
    public static event Action DisableBossBar;
    public static event Action<int,int,bool> ChangeBossBar;

    public event Action BossDeath;


    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        EnableEntity();
    }

    public override void EnableEntity()
    {
        base.EnableEntity();

        health = maxHP;
        currentPhase = 0;
        phaseChanging = false;

        if (patternRoutine != null)
            StopCoroutine(patternRoutine);

        patternRoutine = StartCoroutine(PatternLoop());
        ActivateBossBar?.Invoke(killParticleColor, icon, maxHP, health);
    }

    protected virtual void Update()
    {
        TurnToTarget();
        HandlePhases();
    }

    protected virtual void FixedUpdate()
    {
        FollowTarget();
    }

    protected virtual void FollowTarget()
    {
        if (target == null || isInPattern) return;

        if (rb.linearVelocity.magnitude > 0.1f && !isInPattern)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, (target.transform.position - transform.position).normalized * speed, 0.1f);
        rb.position += rb.linearVelocity * Time.deltaTime;
    }

    // ------------------ PHASE SYSTEM ------------------
    protected virtual void HandlePhases()
    {
        float hpPercent = (float)health / (float)maxHP;

        int newPhase = GetPhaseFromHP(hpPercent);

        if (newPhase != currentPhase && !phaseChanging)
        {
            StartCoroutine(ChangePhase(newPhase));
        }
    }



    protected virtual int GetPhaseFromHP(float hpPercent)
    {
        if (hpPercent > 0.6f) return 0;
        if (hpPercent > 0.3f) return 1;
        return 2;
    }

    protected virtual IEnumerator ChangePhase(int newPhase)
    {
        phaseChanging = true;

        OnPhaseEnd(currentPhase);

        yield return new WaitForSeconds(0.5f);

        currentPhase = newPhase;

        OnPhaseStart(currentPhase);

        phaseChanging = false;
    }

    protected virtual void OnPhaseStart(int phase) { }
    protected virtual void OnPhaseEnd(int phase) { }

    // ------------------ PATTERN LOOP ------------------
    protected virtual IEnumerator PatternLoop()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            if (!isInPattern && !phaseChanging)
            {
                yield return StartCoroutine(DoPattern());
            }

            yield return null;
        }
    }

    protected abstract IEnumerator DoPattern();



    public virtual IEnumerator ChargeAttack(float duration, string triggerName)
    {
        if (animator != null && !string.IsNullOrEmpty(triggerName))
        {
            animator.SetTrigger(triggerName);
        }

        yield return new WaitForSeconds(duration);
    }

    public override void TakeDamage(int damageAmount)
    {
        base.TakeDamage(damageAmount);
        ChangeBossBar?.Invoke(maxHP, health, true);
    }

    public void Heal(int healAmount)
    {
        if (health + healAmount > maxHP)
        {
            health = maxHP;
        }
        else
        {
            health += healAmount;
        }

        ChangeBossBar?.Invoke(maxHP, health, false);
    }

    public override void Die()
    {
        base.Die();
        StopAllCoroutines();

        /*
        ParticleSystem particles = PoolManager.Instance.Get(partPoolKey).GetComponent<ParticleSystem>();
        particles.transform.position = transform.position;
        var main = particles.main;
        main.startColor = killParticleColor;
        particles.Play();
        */

        StopAllCoroutines();

        BossDeath?.Invoke();
        DisableBossBar?.Invoke();

        //PoolManager.Instance.Release(partPoolKey, particles.gameObject, 2f);
        PoolManager.Instance.Release(poolKey, gameObject);
    }



    public virtual Boss SpawnBoss(BossRoom room)
    {
        SetPoolKeys();
        GameObject bossObj = PoolManager.Instance.Get(poolKey);

        if (!bossObj.activeSelf)
        {
            Debug.LogWarning($"Object from pool '{poolKey}' was not active! Forcing activation.");
            bossObj.SetActive(true);
        }

        bossObj.transform.position = room.spawnPos.position;
        Boss boss = bossObj.GetComponent<Boss>();

        if (boss == null)
        {
            Debug.LogError("Spawned object missing Enemy component!");
            return null;
        }

        return boss;
    }
}
