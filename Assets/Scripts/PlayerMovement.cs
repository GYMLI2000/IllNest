using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed;
    private InputAction moveAction;
    private Rigidbody2D rb;
    private Vector2 moveValue;

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        rb = GetComponent<Rigidbody2D>();
    }

    private void Move()
    {
        rb.MovePosition(rb.position +moveValue * movementSpeed * Time.deltaTime);
    }

    private void Update()
    {
        moveValue = moveAction.ReadValue<Vector2>().normalized;
    }

    private void FixedUpdate()
    {
        Move();
    }
}
