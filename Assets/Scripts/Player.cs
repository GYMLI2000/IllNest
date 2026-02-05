using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.EventTrigger;


public class Player : MonoBehaviour
{
    public event Action<int> changeHp;
    public event Action<int> changeMaxHp;

    [SerializeField]
    private DebuffManager debuffManager;


    public float movementSpeed;
    private InputAction moveAction;
    public Rigidbody2D rb;
    private Vector2 moveValue;
    private bool isAttacking;
    private Vector2 aimDir;

    private PlayerControls playerControls;
    public SpriteRenderer[] spriteRenderer;
    public Animator animator { get; private set; }

    [SerializeField]
    private Transform gloveSprite;
    [SerializeField]
    private SpriteRenderer gloveOpen;
    [SerializeField] 
    private SpriteRenderer gloveClosed;

    [SerializeField]
    private int currentHp;
    [SerializeField]
    private int maxHp;
    [SerializeField]
    private float range;
    public float projSpeed;
    public float atkCooldown;
    private float lastAtk;
    [SerializeField]
    public int damage;
    [SerializeField]
    private float invFrames;
    private bool isStaggered;
    [SerializeField]
    private float knockback;
    private float lastHit = 0;
    public int passThrough = 0;

    [SerializeField]
    private string projectileKey;

    [SerializeField]
    private Transform firepointCenter;
    [SerializeField]
    private Transform firepoint;

    public int shootAngle = 1;


    private bool isInvincible = false;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        moveAction = playerControls.Player.Move;


        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();

        gloveOpen.enabled = false;

        currentHp = maxHp;
        changeMaxHp?.Invoke(maxHp);
        changeHp?.Invoke(currentHp);
    }

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.Player.Attack.performed += context => isAttacking = true;
        playerControls.Player.Attack.canceled += context => isAttacking = false;

    }

    private void OnDisable()
    {
        playerControls.Player.Attack.performed -= context => isAttacking = true;
        playerControls.Player.Attack.canceled -= context => isAttacking = false;

        playerControls.Disable();
    }


    private void Attack()
    {
        if (Time.time <= lastAtk + atkCooldown || !isAttacking)
            return;

        lastAtk = Time.time;

        StartCoroutine(AttackEffect());

        GameObject projectile = PoolManager.Instance.Get(projectileKey);
        projectile.GetComponentInChildren<Projectile>().SetStats(
            firepoint.position
            , damage,
            Quaternion.Euler(0, 0, shootAngle) * firepoint.right
            , projSpeed
            , false
            , range
            , gameObject,
            knockback,
            passThrough);
        projectile.transform.position = firepoint.position;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            var enemy = collision.gameObject.GetComponent<Enemy>();
            TakeHit(enemy, enemy.damage, enemy.knockback);
        }
    }

    public void TakeHit(Enemy enemy, int damage, float knockback)
    {
        TakeDamage(damage);

        isStaggered = true;
        Vector2 knockbackDirection = (transform.position - enemy.transform.position).normalized;
        rb.AddForce(knockbackDirection * knockback, ForceMode2D.Impulse);
    }

    private void MoveFirePoint()
    {
        Vector2 delta =  Mouse.current.delta.ReadValue() * 0.01f; // posun mysi

        aimDir += delta;
        aimDir = aimDir.normalized; 

        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        // otaceni hrace podle toho kam míøí
        if (angle > 90f || angle < -90f)
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        else
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);


        if (angle >= -50f && angle <= 50f) //otaceni spritu rukavice
        {
            gloveSprite.localScale = new Vector3(1f, 1f, 1f);
            firepointCenter.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        else if ((angle >= 130f && angle <= 180f) || (angle <= -130f && angle >= -180f))
        {
            gloveSprite.localScale = new Vector3(-1f, 1f, 1f);
            firepointCenter.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    public bool TakeDamage(int damage)
    {
        if (Time.time <= lastHit + invFrames) return false;

        if (isInvincible) // tohle potom zmenit jenom pro ukazku abych neumiral
        {
            damage = 0;
        }
        
        currentHp -= damage;


        StartCoroutine(HitEffect());


        if (currentHp <= 0) 
        { 
            Debug.Log("Jses mrtvej");
            Die();
        }


        changeHp?.Invoke(currentHp);
        lastHit = Time.time;
        return damage > 0;
    }

    private void Die()
    {
        animator.SetTrigger("Die");
        StopAllCoroutines();
        foreach (SpriteRenderer s in spriteRenderer)
        {
            s.color = Color.white;
        }
        DeathScreen.Instance.Show();
        rb.simulated = false;
        gloveSprite.rotation = Quaternion.Euler(0f, 90f, 0f);
        debuffManager.ClearDebuffs();
        debuffManager.enabled = false;
        this.enabled = false;
    }

    private IEnumerator HitEffect()
    {
        foreach (SpriteRenderer s in spriteRenderer)
        {
            s.color = Color.red;
        }
        
        

        yield return new WaitForSeconds(0.1f);


        foreach (SpriteRenderer s in spriteRenderer)
        {
            s.color = Color.white;
        }
        
    }

    private IEnumerator AttackEffect()
    {
        gloveOpen.enabled = true;
        gloveClosed.enabled = false;

        yield return new WaitForSeconds(0.4f);

        gloveOpen.enabled = false;
        gloveClosed.enabled = true;
    }

    private void Move()
    {
        if (!isStaggered)
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, moveValue * movementSpeed, 0.1f);
            rb.position += rb.linearVelocity * Time.deltaTime;
        }
        else if (Time.time + invFrames >= lastHit)
        {
            isStaggered = false;
        }

    }

    private void Update()
    {
        moveValue = moveAction.ReadValue<Vector2>().normalized;

        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            Debug.Log("Invincibility toggled"); // tohle potom zmenit jenom pro ukazku abych neumiral
            isInvincible = !isInvincible; // tohle potom zmenit jenom pro ukazku abych neumiral
        }

        if (moveValue.magnitude > 0)
        {
            animator.SetBool("isWalking",true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        MoveFirePoint();
        Attack();
    }

    private void FixedUpdate()
    {
        Move();
    }
}
