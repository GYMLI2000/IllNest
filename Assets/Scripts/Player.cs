using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


public class Player : MonoBehaviour
{
    public event Action<int> changeHp;
    public event Action<int> changeMaxHp;



    [SerializeField]
    private float movementSpeed;
    private InputAction moveAction;
    private Rigidbody2D rb;
    private Vector2 moveValue;
    private Vector2 mousePosition;
    private bool isAttacking;
    private Vector2 aimDir;

    private PlayerControls playerControls;
    private SpriteRenderer[] spriteRenderer;
    private Animator animator;

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
    [SerializeField]
    private float projSpeed;
    [SerializeField]
    private float atkCooldown;
    private float lastAtk;
    [SerializeField]
    public int damage;
    [SerializeField]
    private float invFrames;
    private bool isStaggered;
    [SerializeField]
    private float knockback;
    private float lastHit = 0;
    [SerializeField]
    private GameObject projectilePrefab;

    [SerializeField]
    private Transform firepointCenter;
    [SerializeField]
    private Transform firepoint;


    private void Start()
    {
        Cursor.visible = false;
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

        GameObject projectile = PoolManager.Instance.Get("PillProjectile");
        projectile.GetComponentInChildren<Pill>().SetStats(firepoint.position,damage, firepoint.right,projSpeed,false,range);
        projectile.transform.position = firepoint.position;

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            var enemy = collision.gameObject.GetComponent<Enemy>();
            TakeDamage(enemy.damage);

            isStaggered = true;
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            rb.AddForce(knockbackDirection * enemy.knockback, ForceMode2D.Impulse);
        }
    }

    private void MoveFirePoint()
    {
        Vector2 delta =  Mouse.current.delta.ReadValue() * 0.01f;

        aimDir += delta;
        aimDir = aimDir.normalized; 

        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        // otaceni hrace
        if (angle > 90f || angle < -90f)
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        else
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);


        if (angle >= -50f && angle <= 50f) //otaceni rukavice
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

    public void TakeDamage(int damage)
    {
        if (Time.time <= lastHit + invFrames) return;

        currentHp -= damage;

        StartCoroutine(HitEffect());

        if (currentHp <= 0) Debug.Log("Jses mrtvej");


        changeHp?.Invoke(currentHp);
        lastHit = Time.time;
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
            rb.linearVelocity = moveValue * movementSpeed;
        }
        else if (Time.time + invFrames >= lastHit)
        {
            isStaggered = false;
        }

    }

    private void Update()
    {
        moveValue = moveAction.ReadValue<Vector2>().normalized;

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
