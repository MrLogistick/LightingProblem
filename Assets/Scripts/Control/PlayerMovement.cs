using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Horizontal Movement")]
    [SerializeField] float acceleration;
    [SerializeField] float runSpeed;
    float moveSpeed;
    float targetSpeed;
    float horizontal;

    [Header("Vertical Movement")]
    [SerializeField] float jumpPower;
    [SerializeField] float gravity;
    [SerializeField] float fallMultiplier;
    [SerializeField] float apexHang;
    [SerializeField] float coyoteTime;
    float apexElapsed;
    float ctElapsed;

    [Header("References")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    Rigidbody2D rb;

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

        if (rb.linearVelocityY > 0) {
            apexElapsed = 0;
            rb.gravityScale = gravity;
        }
        else if (!IsGrounded()) {
            if (apexElapsed < apexHang) {
                rb.linearVelocityY = 0;
                rb.gravityScale = 0;
                apexElapsed += Time.deltaTime;
            }
            else {
                rb.gravityScale = fallMultiplier;
            }
        }

        targetSpeed = horizontal * runSpeed;
    }

    void FixedUpdate() {
        moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        rb.linearVelocityX = moveSpeed;
    }

    public void Move(InputAction.CallbackContext context) {
        horizontal = context.ReadValue<float>();
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
