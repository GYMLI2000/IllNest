using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using Random = UnityEngine.Random;

public class CancerBoss : Boss
{
    [Header("Boss - Minions")]
    protected List<Enemy> minions = new List<Enemy>();
    protected List<Enemy> tumors = new List<Enemy>();

    protected float absorbCooldown = 15f;
    protected float lastAbsorbTime;

    [SerializeField] private SpriteRenderer[] backgroundSpriteRenderers;
    private Color backgColor = new Color(162f/255f, 114f/255f, 18f/255f);

    public static event Action OnCancerHit;

    protected override IEnumerator DoPattern()
    {
        isInPattern = true;

        float roll = UnityEngine.Random.value;

        if (roll < 0.16f && minions.Count < 7)
            yield return SpawnMinions();
        else if (roll < 0.32f)
            yield return SpawnTumors();
        else if (roll < 0.48f)
            yield return WaveAttack();
        else if (roll < 0.64f)
            yield return WormShotAttack();
        else if (roll < 0.8f)
            yield return SpamAttack();
        else
            yield return AttackSpread();


        if (minions.Count > 0 && Time.time > lastAbsorbTime + absorbCooldown)
        {
            yield return Absorb();
            lastAbsorbTime = Time.time;
        }

        isInPattern = false;
    }

    public override void TakeDamage(int damageAmount)
    {
        base.TakeDamage(damageAmount);
        OnCancerHit?.Invoke();
    }

    private IEnumerator DeathCoroutine()
    {
        animator.SetTrigger("Death");
        AudioManager.Instance.StopMusic();

        AudioManager.Instance.PlaySFX("CancerDeath");
        yield return new WaitForSeconds(6f);

        AudioManager.Instance.StartMusicSystem();
        base.Die();
    }

    private IEnumerator WormShotAttack()
    {
        yield return StartCoroutine(ChargeAttack(1f, "BurstAttackCharge"));
        Vector2 baseDir = (target.transform.position - transform.position).normalized;

        AudioManager.Instance.PlaySFX("CancerShortScream");
        animator.SetTrigger("BurstAttack");
        for (int j = 0; j < 10; j++) 
        { 
            for (int i = -1; i <= 1; i++)
            {

                Vector2 rotatedDir = Quaternion.Euler(0, 0, 45f * i) * baseDir;

                GameObject projObj = PoolManager.Instance.Get("CancerProjectile2");
                var projectile = projObj.GetComponentInChildren<CancerProjectile2>();

                projectile.SetStats(firepoint.position, damage, rotatedDir, 5f, true, 30, gameObject, knockback, 0, 3);


                projObj.transform.position = firepoint.position;
            }
            yield return new WaitForSeconds(0.06f);

        }


        yield return new WaitForSeconds(0.5f * (2-currentPhase));
    }

    protected override void Update()
    {
        base.Update();
        foreach (var sr in backgroundSpriteRenderers)
        {
            sr.color = backgColor;
        }
        //DebugCurrentAnimation();
    }
    private void DebugCurrentAnimation()
    {
        if (animator == null)
        {
            Debug.LogError($"[Boss Debug] {gameObject.name}: Animator reference is NULL!");
            return;
        }

        // Get info about the base layer (index 0)
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);

        string clipName = clipInfo.Length > 0 ? clipInfo[0].clip.name : "No Clip Found";

        // stateInfo.fullPathHash or stateInfo.shortNameHash can also be used, 
        // but name is best for debugging.
        Debug.Log($"[Boss Debug] {gameObject.name} | State: {clipName} | Normalized Time: {stateInfo.normalizedTime}");

        if (animator.speed == 0)
        {
            Debug.LogWarning($"[Boss Debug] {gameObject.name}: Animator speed is set to 0!");
        }
    }

    private IEnumerator WaveAttack()
    {
        animator.SetBool("WaveAttack", true);
        int numberOfWaves = 6;
        int projectilesPerRow = 14;
        float timeBetweenWaves = 2.0f;

        float spawnDelay = 0.08f;

        Vector2 origin = firepoint.position;
        float horizontalReach = 16f;

        float roomLeftX = origin.x - horizontalReach;
        float roomRightX = origin.x + horizontalReach;

        float roomTopY = origin.y - 1f;
        float roomBottomY = origin.y - 25f;

        string waveProjKey = "CancerProjectile2";

        AudioManager.Instance.PlaySFX("CancerLongScream");
        for (int wave = 0; wave < numberOfWaves; wave++)
        {
            bool spawnFromLeft = (wave % 2 == 0);

            Vector2 travelDir = spawnFromLeft ? Vector2.right : Vector2.left;
            float spawnY = Random.Range(roomBottomY + 3f, roomTopY - 3f);

            Vector2 spawnPos = new Vector2(spawnFromLeft ? roomLeftX : roomRightX, spawnY);

            animator.SetTrigger("BurstAttack");
            for (int p = 0; p < projectilesPerRow; p++)
            {
                GameObject projObj = PoolManager.Instance.Get(waveProjKey);
                var projectile = projObj.GetComponentInChildren<CancerProjectile2>();

                if (projectile != null)
                {
                    projectile.SetStats(
                        spawnPos,             
                        damage,
                        travelDir,
                        4.5f,
                        true,
                        80f,
                        gameObject,
                        knockback,
                        -1,
                        6f                
                    );
                }


                yield return new WaitForSeconds(spawnDelay);
            }

            float timeTakenToSpawnRow = projectilesPerRow * spawnDelay;
            float actualWaitTime = Mathf.Max(0, timeBetweenWaves - timeTakenToSpawnRow);

            yield return new WaitForSeconds(actualWaitTime);
        }

        animator.SetBool("WaveAttack", false);

        yield return new WaitForSeconds(1.5f * (2-currentPhase));
    }
    private IEnumerator AttackSpread()
    {
        yield return StartCoroutine(ChargeAttack(1f, "BurstAttackCharge"));

        int projectileCount = Random.Range(4, 8);
        float spreadAngle = 60f;

        Vector2 baseDir = (target.transform.position - transform.position).normalized;
        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;

        animator.SetTrigger("BurstAttack");
        AudioManager.Instance.PlaySFX("CancerAttack");
        for (int i = 0; i < projectileCount; i++)
        {
            float angleOffset = Random.Range(-spreadAngle / 2f, spreadAngle / 2f);
            float finalAngle = baseAngle + angleOffset;

            Vector2 dir = new Vector2(
                Mathf.Cos(finalAngle * Mathf.Deg2Rad),
                Mathf.Sin(finalAngle * Mathf.Deg2Rad)
            );

            GameObject projObj = PoolManager.Instance.Get(projKey);
            projObj.transform.position = firepoint.position + (Vector3)Random.insideUnitCircle;

            var projectile = projObj.GetComponentInChildren<CancerProjectile>();

            float randomSize = Random.Range(2f, 3.5f);
            float randomSpeed = Random.Range(4f, 7f);
            float randomRange = Random.Range(10f, 20f);

            projectile.SetStats(
                firepoint.position,
                damage,
                dir,
                randomSpeed,
                true,
                randomRange,
                gameObject,
                knockback,
                0,
                randomSize
            );
        }

        yield return new WaitForSeconds(0.5f * (2-currentPhase));
    }

    private IEnumerator Absorb()
    {
        animator.SetBool("isAbsorbing",true);
        yield return new WaitForSeconds(1f);

        AudioManager.Instance.PlaySFX("CancerAbsorb");
        while (minions.Count > 0)
        {
            for (int i = minions.Count - 1; i >= 0; i--)
            {
                Enemy minion = minions[i];

                minion.transform.position = Vector3.MoveTowards(
                    minion.transform.position,
                    transform.position,
                    10 * Time.deltaTime
                );

                float distance = Vector2.Distance(minion.transform.position, transform.position);

                if (distance <= 4)
                {
                    minion.Die(false);
                    minions.RemoveAt(i);
                    Heal(20);          
                }
            }

            yield return null;
        }
        animator.SetBool("isAbsorbing", false);

        yield return new WaitForSeconds(3f* (2-currentPhase));
    }


    private IEnumerator SpamAttack()
    {
        yield return StartCoroutine(ChargeAttack(1f, "Charge"));

        int projectileCount = 10;

        Vector2 baseDir = (target.transform.position - transform.position).normalized;

        for (int i = 0; i < projectileCount; i++)
        {
            AudioManager.Instance.PlaySFX("CancerAttack");
            animator.SetTrigger("Attack");
            baseDir = (target.transform.position - transform.position).normalized;


            GameObject projObj = PoolManager.Instance.Get("CancerProjectile");
            projObj.transform.position = firepoint.position;
            var projectile = projObj.GetComponentInChildren<CancerProjectile>();

            float randomSize = Random.Range(2f, 3.5f);
            float randomSpeed = Random.Range(7f, 14f);
            float randomRange = Random.Range(25f, 40f);

            projectile.SetStats(
                firepoint.position,
                damage,
                baseDir,
                randomSpeed,
                true,
                randomRange,
                gameObject,
                knockback,
                0,
                randomSize
            );
            yield return new WaitForSeconds(1f);

        }

        yield return new WaitForSeconds(0.5f * (2-currentPhase));

    }

    private IEnumerator SpawnMinions()
    {
        lastAbsorbTime = Time.time;
        yield return StartCoroutine(ChargeAttack(1f, "SummonMinions"));

        int minionCount = 3;
        AudioManager.Instance.PlaySFX("CancerSpawnMinion");

        for (int i = 0; i < minionCount; i++)
        {
            float randomX = Random.Range(-5f, 5f);

            float randomY = Random.Range(-8f, -3f);

            Vector3 spawnOffset = new Vector3(randomX, randomY, 0);
            Vector3 pos = firepoint.position + spawnOffset;

            PoolManager.Instance.Get("CancerMinion", 0.05f, minionObj =>
            {
                minionObj.transform.position = pos;
                var clone = minionObj.GetComponent<CancerMinionEnemy>();
                clone.EnableEnemy();
                RegisterMinion(clone);
            });
        }

        yield return new WaitForSeconds(2f* (2-currentPhase));
    }

    protected override void FollowTarget()
    {

    }

    private IEnumerator SpawnTumors()
    {
        yield return StartCoroutine(ChargeAttack(1f, "SummonMinions"));

        int tumorCount = 6;

        int maxAttempts = 30;       
        float checkRadius = 1.0f;     
        float roomWidth = 12f;        
        float dropDepth = 16f;       

        Vector2 origin = firepoint.position;

        AudioManager.Instance.PlaySFX("CancerSummonTumor");
        for (int i = 0; i < tumorCount; i++)
        {
            Vector3 finalSpawnPos = origin;
            bool spotFound = false;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {

                float randomX = Random.Range(-roomWidth, roomWidth);
                float randomY = Random.Range(-dropDepth, -2f);

                Vector2 testPos = new Vector2(origin.x + randomX, origin.y + randomY);


                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(testPos, checkRadius);
                bool hitObstacle = false;


                foreach (Collider2D col in hitColliders)
                {
                    if (col.CompareTag("Wall") || col.CompareTag("Enemy"))
                    {
                        hitObstacle = true;
                        break; 
                    }
                }


                if (!hitObstacle)
                {
                    finalSpawnPos = testPos;
                    spotFound = true;
                    break; 
                }
            }

            if (!spotFound)
            {
                Debug.LogWarning($"[CancerBoss] Room too crowded! Tumor {i} couldn't find a spot.");
            }
            else
            {
                Vector3 posToSpawn = finalSpawnPos;

                PoolManager.Instance.Get("CancerTumor", 0.05f, minionObj =>
                {
                    minionObj.transform.position = posToSpawn;
                    var clone = minionObj.GetComponent<CancerTumor>();

                    if (clone != null)
                    {
                        clone.EnableEnemy();
                        tumors.Add(clone);
                    }
                });
            }
        }

        yield return new WaitForSeconds(2f* (2-currentPhase));
    }

    protected override void SetPoolKeys()
    {
        poolKey = "CancerBoss";
        projKey = "CancerProjectile";
    }


    protected override void InitializeStats()
    {
        killParticleColor = new Color(97f/255f, 128f/255f, 80f/255f, 0.2f);
        maxHP = 500;
        health = 500;
        damage = 2;
        speed = 7;
        knockback = 2f;
        knockbackReduction = 1;
        turns = false;
        looksATarget = false;

    }


    protected virtual void RegisterMinion(Enemy m)
    {
        if (!minions.Contains(m))
            minions.Add(m);
    }

    protected virtual void RemoveMinion(Enemy m)
    {
        if (minions.Contains(m))
            minions.Remove(m);
    }

    protected virtual void ClearMinions()
    {
        foreach (var m in minions)
        {
            if (m != null)
                m.Die();
        }

        foreach (var t in tumors)
        {
            if (t != null)
                t.Die();
        }
        minions.Clear();
        tumors.Clear();
    }

    public override void Die()
    {
        ClearMinions();
        StopAllCoroutines();
        StartCoroutine(DeathCoroutine());
    }
}
