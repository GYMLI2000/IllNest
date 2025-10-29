using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public event Action<int> changeHp;
    public event Action<int> changeMaxHp;



    [SerializeField]
    private float movementSpeed;
    private InputAction moveAction;
    private Rigidbody2D rb;
    private Vector2 moveValue;
    private bool isAttacking;

    private PlayerControls playerControls;

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
    private int damage;
    [SerializeField]
    private GameObject projectilePrefab;

    

    private void Start()
    {
        moveAction = playerControls.Player.Move;
        rb = GetComponent<Rigidbody2D>();

        currentHp = maxHp;
        changeHp?.Invoke(currentHp);
        changeMaxHp?.Invoke(maxHp);
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
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());


        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponentInChildren<Pill>().SetStats(damage, (mousePosition - (Vector2)transform.position).normalized,projSpeed,false,range);


    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;

        if (currentHp < 0) Debug.Log("Jses mrtvej");


        changeHp?.Invoke(currentHp);
    }

    private void Move()
    {
        rb.MovePosition(rb.position +moveValue * movementSpeed * Time.deltaTime);
    }

    private void Update()
    {
        moveValue = moveAction.ReadValue<Vector2>().normalized;
        Attack();
    }

    private void FixedUpdate()
    {
        Move();
    }
}
