using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class SchizophreniaBoss : Boss
{
    [SerializeField]
    private List<GameObject> mirrorClones = new List<GameObject>();
    private List<Animator> cloneAnimators = new List<Animator>();

    private bool clonesActive = false;
    private float clonesRealness = 0f;

    public void SpawnMirrorClones()
    {
        if (clonesActive) return;

        AudioManager.Instance.PlaySFX("SchizoClone");

        foreach (var clone in mirrorClones)
        {
            if (clone != null)
            {
                clone.SetActive(true);
            }


            Animator cloneAnim = clone.GetComponent<Animator>();
            if (cloneAnim != null)
            {
                cloneAnimators.Add(cloneAnim);
            }
        }
        clonesActive = true;
    }

    protected virtual void LateUpdate()
    {
        if (clonesActive && target != null && mirrorClones.Count == 3)
        {
            Vector3 playerPos = target.transform.position;
            Vector3 bossOffset = transform.position - playerPos;

            if (mirrorClones[0] != null)
                mirrorClones[0].transform.position = playerPos + new Vector3(-bossOffset.x, bossOffset.y, bossOffset.z);

            if (mirrorClones[1] != null)
            {
                mirrorClones[1].transform.position = playerPos + new Vector3(bossOffset.x, -bossOffset.y, bossOffset.z);
            }

            if (mirrorClones[2] != null)
            {
                mirrorClones[2].transform.position = playerPos + new Vector3(-bossOffset.x, -bossOffset.y, bossOffset.z);
            }


            float mainBossScaleX = Mathf.Sign(transform.localScale.x);

            foreach (var clone in mirrorClones)
            {
                if (clone != null)
                {
                    float desiredCloneDir = (clone.transform.position.x < playerPos.x) ? -1f : 1f;

                    clone.transform.localScale = new Vector3(desiredCloneDir * mainBossScaleX, 1, 1);
                }
            }
            UpdateClonesAlpha();

        }
    }


    private void SyncAnimTrigger(string triggerName)
    {
        animator.ResetTrigger(triggerName);
        animator.SetTrigger(triggerName);
        foreach (var anim in cloneAnimators)
        {
            if (anim != null)
            {
                anim.ResetTrigger(triggerName);
                anim.SetTrigger(triggerName);
            }
        }
    }

    private void SyncAnimBool(string boolName, bool state)
    {
        animator.SetBool(boolName, state);
        foreach (var anim in cloneAnimators)
        {
            if (anim != null) anim.SetBool(boolName, state);
        }
    }

    public override void TakeDamage(int damageAmount)
    {
        base.TakeDamage(damageAmount);
        clonesRealness = (float)health / (float)maxHP;
    }

    private void UpdateClonesAlpha()
    {
        if (maxHP <= 0 || !clonesActive) return;

        clonesRealness = Mathf.Clamp01((float)health / maxHP);

        // When HP is full (realness 1), alpha is 0.1f. As HP drops to 0, alpha goes to 1f.
        float alpha = Mathf.Lerp(1f, 0.1f, clonesRealness);

        foreach (var clone in mirrorClones)
        {
            if (clone != null)
            {
                foreach (var sr in clone.GetComponentsInChildren<SpriteRenderer>())
                {
                    if (sr != null)
                    {
                        Color color = sr.color;
                        color.a = alpha;
                        sr.color = color;
                    }
                }
            }
        }
       
    }

    protected override IEnumerator DoPattern()
    {
        yield return new WaitForSeconds(2f);

        isInPattern = true;

        float roll = UnityEngine.Random.value;

        if (roll < 0.20f)
            yield return DashAttack();
        else if (roll < 0.40f)
            yield return ShotAttack();
        else if (roll < 0.60f)
            yield return MeleeAttack();
        else if (roll < 0.80f)
            yield return AreaAttack();
        else
            yield return HauntAttack(); 


        isInPattern = false;
    }

    private IEnumerator Dash(float dashTime, float dashForce, Vector3 dir)
    {
        Vector2 dashDir = dir;
        rb.AddForce(dashDir * dashForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(dashTime);
    }

    private IEnumerator DashAttack()
    {
        SyncAnimBool("isWalking", false);
        SyncAnimTrigger("Dash");
        yield return new WaitForSeconds(1f);

        AudioManager.Instance.PlaySFX("SchizoDash");


        Vector2 dashDir = ((target.transform.position- Vector3.down) - transform.position).normalized;
        AudioManager.Instance.PlaySFX("SchizoDash");
        yield return new WaitForSeconds(0.6f);
        SyncAnimTrigger("Dash");
        yield return Dash(2f, 150f,dashDir);


    }

    private IEnumerator HauntAttack()
    {
        SyncAnimBool("isWalking", false);
        SyncAnimTrigger("Haunt");
        yield return new WaitForSeconds(1f);

        AudioManager.Instance.PlaySFX("SchizoHauntDeath");

        int maxHaunts = 10;
        float spawnDistance = 10f;

        SyncAnimBool("isHaunting",true);

        for (int i = 0; i < maxHaunts; i++)
        {
            GameObject activeHaunt = null;

            Vector2 randomDir = UnityEngine.Random.insideUnitCircle.normalized;
            Vector3 spawnPosition = transform.position + (Vector3)(randomDir * spawnDistance);

            PoolManager.Instance.Get("SchizophreniaHaunt", 0f, minionObj =>
            {
                activeHaunt = minionObj;
                minionObj.transform.position = spawnPosition;

                var haunt = minionObj.GetComponent<SchizophreniaHaunt>();
                haunt.EnableEnemy();
                haunt.boss = this;
            });

            yield return new WaitUntil(() => activeHaunt != null);

            yield return new WaitUntil(() => !activeHaunt.activeInHierarchy);

            yield return new WaitForSeconds(0.1f);
        }
        SyncAnimBool("isHaunting", false);

        yield return new WaitForSeconds(2f);
    }

    private IEnumerator AreaAttack()
    {
        SyncAnimBool("isWalking", false);
        SyncAnimTrigger("Area");

        knockbackReduction = 1;

        AudioManager.Instance.PlaySFX("SchizoArea");

        yield return new WaitForSeconds(1.5f);



        int totalShots = 60;      
        float delayBetweenShots = 0.1f;

        SyncAnimBool("isShooting", true);


        for (int i = 0; i < totalShots; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Vector2 randomDir = UnityEngine.Random.insideUnitCircle.normalized;

                GameObject projObj = PoolManager.Instance.Get(projKey);
                var projectile = projObj.GetComponentInChildren<SchizophreniaProjectile>();

                projectile.SetStats(firepoint.position, 1, randomDir, 5f, true, -1, gameObject, knockback, 0, 3);
                projObj.transform.position = firepoint.position;
            }
            yield return new WaitForSeconds(delayBetweenShots);
        }


        SyncAnimBool("isShooting", false);

        yield return new WaitForSeconds(2f);


        knockbackReduction = 0;

    }


    private IEnumerator MeleeAttack()
    {
        SyncAnimBool("isWalking", false);
        SyncAnimTrigger("Melee");
        yield return new WaitForSeconds(0.5f);


        Vector2 dashDir = ((target.transform.position- Vector3.down) - transform.position).normalized;

        float roll = UnityEngine.Random.value;


        yield return new WaitForSeconds(0.1f);
        SyncAnimTrigger("Melee");
        AudioManager.Instance.PlaySFX("SchizoMelee1");
        yield return Dash(0.6f, 70f, dashDir);

        if (roll < 0.8f)
        {
            dashDir = ((target.transform.position- Vector3.down) - transform.position).normalized;
            yield return new WaitForSeconds(0.3f);
            SyncAnimTrigger("Melee");
            AudioManager.Instance.PlaySFX("SchizoMelee2");
            yield return Dash(0.6f, 50f, dashDir);
        }
        if (roll < 0.6f)
        {
            dashDir = ((target.transform.position- Vector3.down) - transform.position).normalized;
            yield return new WaitForSeconds(0.3f);
            SyncAnimTrigger("Melee");
            AudioManager.Instance.PlaySFX("SchizoMelee1");
            yield return Dash(0.6f, 50f, dashDir);
        }

        SyncAnimTrigger("Idle");
        yield return new WaitForSeconds(1f);

    }

    private IEnumerator ShotAttack()
    {
        SyncAnimBool("isWalking" , false);
        SyncAnimTrigger("Shot");
        yield return new WaitForSeconds(1f);


        Vector2 baseDir = (target.transform.position - transform.position).normalized;
        AudioManager.Instance.PlaySFX("SchizoShoot");
        SyncAnimTrigger("Shot");

        for (int i = -2; i <= 2; i++)
        {

            Vector2 rotatedDir = Quaternion.Euler(0, 0, 30f * i) * baseDir;

            GameObject projObj = PoolManager.Instance.Get(projKey);
            var projectile = projObj.GetComponentInChildren<SchizophreniaProjectile>();

            projectile.SetStats(firepoint.position, 1, rotatedDir, 5f, true, -1, gameObject, knockback, 0, 3);


            projObj.transform.position = firepoint.position;
        }

        yield return new WaitForSeconds(2f);
    }

    private IEnumerator Attack()
    {
        yield return MeleeAttack();

    }

    protected override void OnPhaseStart(int phase)
    {
        switch (phase)
        {
            case 1:
                SpawnMirrorClones();
                break;
            case 2:
                AddShadowsToClones();
                break;
        }
    }

    private void AddShadowsToClones()
    {
        foreach (var clone in mirrorClones)
        {
            if (clone != null)
            {
                var shadow = clone.transform.GetChild(0);
                shadow.gameObject.SetActive(true);
            }
        }
    }

    /*  Uz nepouzivam ig

public void AddClone(SchizophreniaClone clone)
{
    clones.Add(clone);
}

private IEnumerator Clone()
{
    isInPattern = true;

    animator.SetTrigger("Clone");
    yield return new WaitForSeconds(0.8f);

    PoolManager.Instance.Get("SchizophreniaClone", 0.05f, minionObj =>
    {
        minionObj.transform.position = transform.position + (((target.transform.position.x - transform.position.x) > 0 ? Vector3.right : Vector3.left)*2);
        var clone = minionObj.GetComponent<SchizophreniaClone>();
        clone.EnableEnemy();
        clone.boss = this; 
        AddClone(clone);
    });

    yield return new WaitForSeconds(2);

    isInPattern = false;

}
    */
    protected override void InitializeStats()
    {
        maxHP = 200;
        health = 200;
        damage = 2;
        speed = 6f;
        knockback = 2f;
        knockbackReduction = 0.5f;
        killParticleColor = new Color(120f/255f, 152f/255f, 209f/255f, 0.2f);
        StartCoroutine(Wait());
    }

    protected override void SetPoolKeys()
    {
        poolKey = "SchizophreniaBoss";
        projKey = "SchizophreniaProjectile";
        entityId = 601;
    }

    public override void Die()
    {
        StopAllCoroutines();
        isInPattern = true;
        mirrorClones.Clear();

        SyncAnimBool("isWalking", false);
        SyncAnimBool("isShooting", false);
        SyncAnimBool("isHaunting", false);

        CompletionManager.Instance.CheckCompletion("3504", 1, 1);

        StartCoroutine(DeathCoroutine());
    }

    private IEnumerator Wait()
    {
        isInPattern = true;
        yield return new WaitForSeconds(3f);
        isInPattern = false;
    }

    private IEnumerator DeathCoroutine()
    {
        animator.Play("Idle");
        animator.SetTrigger("Death");
        AudioManager.Instance.PlaySFX("SchizoDeath");
        foreach (var cloneAnim in cloneAnimators)
        {
            if (cloneAnim != null)
            {
                cloneAnim.Play("Idle");
                cloneAnim.SetTrigger("CloneDeath");
            }
        }

        yield return new WaitForSeconds(6f);
        base.Die();
    }
}
