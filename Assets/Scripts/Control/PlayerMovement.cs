using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Horizontal Movement")]
    float moveSpeed;
    float targetSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    float horizontal;

    [Header("Vertical Movement")]
    [SerializeField] float jumpPower;
    [SerializeField] float gravity;
    [SerializeField] float fallMultiplier;
    [SerializeField] float coyoteTime;
    float ctElapsed;

    [Header("References")]
    Rigidbody2D rb;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        if (IsGrounded()) {
            ctElapsed = coyoteTime;
        }
        else {
            ctElapsed -= Time.deltaTime;
        }
    }

    void FixedUpdate() {
        rb.AddForceX(acceleration * horizontal, ForceMode2D.Force);
    }

    public void Move(InputAction.CallbackContext context) {
        horizontal = context.ReadValue<float>();
    }

    public void Sprint(InputAction.CallbackContext context) {
        if (context.performed) {
            targetSpeed = runSpeed;
        }
        if (context.canceled) {
            targetSpeed = walkSpeed;
        }
    }

    public void Jump(InputAction.CallbackContext context) {
        if (context.performed && ctElapsed > 0) {
            rb.linearVelocityY = jumpPower;
            ctElapsed = 0;
        }
        if (context.canceled && rb.linearVelocityY > 0) {
            rb.linearVelocityY *= 0.5f;
        }
    }

    bool IsGrounded() {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
}
