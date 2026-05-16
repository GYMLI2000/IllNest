using System;
using System.Collections;
using UnityEngine;

public abstract class HostileEntity : MonoBehaviour
{
    protected int entityId;

    [Header("Health / Core Stats")]
    [HideInInspector] public int health;
    [HideInInspector] public float speed { get; protected set; }
    [HideInInspector] public int damage { get; protected set; }
    [HideInInspector] public float knockback;
    [HideInInspector] public float knockbackReduction = 0f;

    [Header("Targeting")]
    [HideInInspector] public GameObject target;
    [HideInInspector] public Vector2 targetPosition;
    protected bool looksATarget = true;
    protected bool turns = true;

    [Header("Combat Visuals")]
    public Transform firepoint; 

    [Header("Movement / Physics")]
    public Rigidbody2D rb { get; protected set; }

    [Header("Projectiles / Pools")]
    protected string partPoolKey = "EnemyDeath";
    public string poolKey { get; protected set; }
    public string projKey { get; protected set; }

    [Header("Visuals / Animation")]
    public Animator animator;
    [HideInInspector] public SpriteRenderer[] spriteRenderers;
    protected Color killParticleColor = Color.white;

    public static event Action<HostileEntity> EntityDeath;


    protected virtual void Awake() { }

    public virtual void EnableEntity()
    {
        SetPoolKeys();
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player");
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        foreach (var sr in spriteRenderers)
        {
            sr.color = Color.white;
        }

        InitializeStats();

        animator = GetComponent<Animator>();
        ResetAnimator();

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    protected abstract void SetPoolKeys();
    protected void ResetAnimator()
    {
        if (animator == null) return;

        animator.Rebind();
        animator.Update(0f);
        foreach (var p in animator.parameters)
        {
            switch (p.type)
            {
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(p.name, false);
                    break;
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(p.name, 0f);
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(p.name, 0);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    animator.ResetTrigger(p.name);
                    break;
            }
        }
    }

    protected void TurnToTarget()
    {
        if (target != null && looksATarget)
        {
            targetPosition = target.transform.position;
            FlipTowards(targetPosition.x);
        }
        else if (targetPosition != null && looksATarget)
        {
            FlipTowards(targetPosition.x);
        }
        else if (!looksATarget && turns)
        {
            if (rb.linearVelocityX > 0)
                transform.localScale = new Vector3(-1, 1, 1);
            else if (rb.linearVelocityX < 0) 
                transform.localScale = new Vector3(1, 1, 1);
        }
    }

    protected virtual void FlipTowards(float targetX)
    {
        if (transform.position.x < targetX)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }

    protected IEnumerator HitEffect(bool didDamage)
    {
        foreach (SpriteRenderer s in spriteRenderers)
        {
            s.color = didDamage ? Color.red : Color.black;
        }

        yield return new WaitForSeconds(0.1f);

        foreach (SpriteRenderer s in spriteRenderers)
        {
            s.color = Color.white;
        }
    }

    public virtual void TakeDamage(int damageAmount)
    {
        health -= damageAmount;

        if (health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(HitEffect(true));
        }
    }

    public virtual void TakeHit(Player player, int damageAmount, float knockbackAmount)
    {
        TakeDamage(damageAmount);

        if (rb != null && player != null)
        {
            Vector2 knockbackDirection = (transform.position - player.transform.position).normalized;
            rb.AddForce(knockbackDirection * (knockbackAmount - knockbackReduction * knockbackAmount), ForceMode2D.Impulse);
        }
    }

    protected abstract void InitializeStats();



    public virtual void Die()
    {
        EntityDeath?.Invoke(this);
        CompletionManager.Instance.DiscoverEntry(entityId);
    }

    public virtual void Die(bool isByPlayer)
    {
    }
}
