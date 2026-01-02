using System.Collections.Generic;
using UnityEngine;

public class OCDEnemy : Enemy
{
    [SerializeField]
    public List<OCDHitpoint> hitpoints;
    public List<Material> enemyMaterial;
    public int axisLock = -1;
    private int currentSequenceIndex = 0;

    public override void EnableEnemy()
    {
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            enemyMaterial.Add(sr.material);
        }
        base.EnableEnemy();
    }

    protected override void SetPoolKeys()
    {
        poolKey = "OCDEnemy";
        projKey = "OCDProjectile";

    }

    protected override void InitializeStats()
    {
        killParticleColor = new Color(86f / 255f, 128f / 255f, 80f /255f, 0.2f);
        chaseRange =  -1;
        damage = 1;
        speed = 2;
        health = 1;
        isAttacking = false;
        attackCooldown = 6;
        attackDuration = 1f;
        chargeTime = 0.7f;
        lastAttack = Time.time;
        attackState = new OCDAttackState(this);
        idleState = new IdleState(this);
        chaseState = new OCDChaseState(this);
        knockback = 1f;
        looksATarget = false;
        turns = false;
        knockbackReduction = 1f;
        AsignHitpoints();
    }




    public override void TakeDamage(int damage)
    {
        //nic
    }

    public void HitHitpoint(int sequenceIndex)
    {
        if(sequenceIndex == currentSequenceIndex)
        {
            hitpoints.Find(hitpoint => hitpoint.sequenceIndex == sequenceIndex).sprite.color = Color.gray;
            currentSequenceIndex++;
            if(currentSequenceIndex >= hitpoints.Count)
            {
                KillEnemy();
            }
        }
        else
        {

            foreach (OCDHitpoint hitpoint in hitpoints)
            {
                hitpoint.sprite.color = hitpoint.hitpointColor;
            }
            currentSequenceIndex = 0;

            if (sequenceIndex == currentSequenceIndex)
            {
                hitpoints.Find(hitpoint => hitpoint.sequenceIndex == sequenceIndex).sprite.color = Color.gray;
                currentSequenceIndex++;
            }
        }

    }

    private void AsignHitpoints()
    {
        List<int> numbers = new List<int>();
        for (int i = 0; i < hitpoints.Count; i++)
        {
            numbers.Add(i);
        }
        
        for (int i = 0; i < hitpoints.Count; i++)
        {
            int randIndex = Random.Range(0, numbers.Count);
            hitpoints[i].sequenceIndex = numbers[randIndex];
            hitpoints[i].sprite.color = hitpoints[i].hitpointColor;
            numbers.RemoveAt(randIndex);
        }
    }
}
