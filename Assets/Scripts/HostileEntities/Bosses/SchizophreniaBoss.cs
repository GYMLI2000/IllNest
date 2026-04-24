using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class SchizophreniaBoss : Boss
{
    private bool cloned = false;
    private List<SchizophreniaClone> clones = new List<SchizophreniaClone>();

    protected override IEnumerator DoPattern()
    {
        yield return new WaitForSeconds(2f);

        isInPattern = true;

        float roll = UnityEngine.Random.value;

        if (roll < 0.16f)
            yield return Attack();
        else if (roll < 0.32f)
            yield return Attack();
        else if (roll < 0.48f)
            yield return Attack();
        else if (roll < 0.64f)
            yield return Attack();
        else if (roll < 0.8f)
            yield return Attack();
        else
            yield return Attack();


        isInPattern = false;
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(0);

    }

    protected override void OnPhaseStart(int phase)
    {
        switch (phase)
        {
            case 1:
                if (!cloned)
                {
                    StartCoroutine(Clone());
                    cloned = true;
                }
                break;
            case 2:
                speed += 5;
                damage += 1;
                break;
        }
    }

    public void AddClone(SchizophreniaClone clone)
    {
        clones.Add(clone);
    }

    private IEnumerator Clone()
    {
        PoolManager.Instance.Get("SchizophreniaClone", 0.05f, minionObj =>
        {
            minionObj.transform.position = transform.position;
            var clone = minionObj.GetComponent<SchizophreniaClone>();
            clone.EnableEnemy();
            clone.boss = this; 
            AddClone(clone);
        });

        yield return new WaitForSeconds(5);
    }

    protected override void InitializeStats()
    {
        maxHP = 200;
        health = 200;
        damage = 2;
        speed = 10;
        knockback = 2f;
        knockbackReduction = 1;
        killParticleColor = new Color(120f/255f, 152f/255f, 209f/255f, 0.2f);
    }

    protected override void SetPoolKeys()
    {
        poolKey = "SchizophreniaBoss";
        projKey = "SchizophreniaProjectile";
    }
}
