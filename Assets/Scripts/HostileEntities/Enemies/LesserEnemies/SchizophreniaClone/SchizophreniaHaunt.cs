using UnityEngine;

public class SchizophreniaHaunt : Enemy
{
    public SchizophreniaBoss boss;


    protected override void InitializeStats()
    {
        killParticleColor = new Color(120f/255f, 152f/255f, 209f/255f, 0.2f);
        chaseRange =  -1;
        damage = 2;
        speed = 10;
        health = 1;
        idleState = new IdleState(this);
        chaseState = new ChaseState(this);
        deathAudio = "SchizoHauntDeath";

        knockback = 2f;
        isLesser = true;

        foreach (var sr in spriteRenderers)
        {
            Color c = sr.color;
            c.a = 0.5f;
            sr.color = c;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TakeDamage(1);
        }
    }

    protected override void SetPoolKeys()
    {
        poolKey = "SchizophreniaHaunt";
        entityId = 401;
    }

    private void LateUpdate()
    {
        foreach (var sr in spriteRenderers)
        {
            Color c = sr.color;
            c.a = 0.5f;
            sr.color = c;
        }
    }
}
