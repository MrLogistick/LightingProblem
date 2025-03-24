using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Basic Movement")]
    float currentSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    float horizontal;

    [SerializeField] float jumpPower;
    [SerializeField] float normalGravity;
    [SerializeField] float fallGravity;
    [SerializeField] float coyoteTime;
    float ctElapsed;

    [Header("Advanced Movement")]
    [SerializeField] float vaultSize;
    [SerializeField] float vaultSpeed;
    [SerializeField] float crouchSpeed;

    [Header("References")]
    Rigidbody2D rb;
    [SerializeField] Vector2 groundCheck;
    [SerializeField] LayerMask groundLayer;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        rb.gravityScale = rb.linearVelocityY < 0 ? fallGravity : normalGravity;

        if (IsGrounded()) {
            ctElapsed = coyoteTime;
        }
        else {
            ctElapsed -= Time.deltaTime;
        }
    }

    void FixedUpdate() {
        rb.linearVelocityX = horizontal * currentSpeed * Time.fixedDeltaTime;
    }

    void Move(InputAction.CallbackContext context) {
        horizontal = context.ReadValue<Vector2>().x;
    }

    void Run(InputAction.CallbackContext context) {
        currentSpeed = context.performed ? runSpeed : walkSpeed;
    }

    void Jump(InputAction.CallbackContext context) {
        if (context.performed && coyoteTime > 0) {
            rb.linearVelocityY = jumpPower;
        }
        if (context.canceled && rb.linearVelocityY > 0) {
            rb.linearVelocityY *= 0.5f;
            ctElapsed = 0;
        }
    }

    bool IsGrounded() {
        return Physics2D.OverlapCircle(groundCheck, 0.1f, groundLayer);
    }

    void Vault(InputAction.CallbackContext context) {

    }

    void Crouch(InputAction.CallbackContext context) {

    }
}
