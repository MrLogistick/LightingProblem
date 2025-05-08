using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Horizontal Movement")]
    [SerializeField] float acceleration;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    bool isRunning;
    float moveSpeed;
    float targetSpeed;
    float horizontal;

    [Header("Vertical Movement")]
    [SerializeField] float jumpPower;
    [SerializeField] float gravity;
    [SerializeField] float fallMultiplier;
    [SerializeField] float apexHang;
    float apexElapsed;
    [SerializeField] float coyoteTime;
    float ctElapsed;
    [SerializeField] float jumpBufferTime;
    float jbElapsed;

    [Header("References")]
    [SerializeField] Transform topCastPosition;
    [SerializeField] Transform centreCastPosition;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    Rigidbody2D rb;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        EnableControls();
    }

    void Update() {
        targetSpeed = horizontal * (isRunning ? runSpeed : walkSpeed);
        jbElapsed -= Time.deltaTime;

        if (IsGrounded()) {
            ctElapsed = coyoteTime;
            apexElapsed = 0;
        }
        else {
            ctElapsed -= Time.deltaTime;
        }

        if (jbElapsed > 0 && ctElapsed > 0) {
            rb.linearVelocityY = jumpPower;
            jbElapsed = 0;
        }

        if (rb.linearVelocityY > 0) {
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
    }

    void FixedUpdate() {
        moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        rb.linearVelocityX = moveSpeed;
    }

    public void Move(InputAction.CallbackContext context) {
        horizontal = context.ReadValue<float>();

        if (horizontal != 0) {
            float dir = Mathf.Sign(horizontal);
            transform.localScale = new Vector3(dir, 1.8f, 1);
        }
    }

    public void Sprint(InputAction.CallbackContext context) {
        if (context.performed) {isRunning = true;}
        if (context.canceled) {isRunning = false;}
    }

    public void Jump(InputAction.CallbackContext context) {
        if (context.performed) {
            jbElapsed = jumpBufferTime;
        }
        if (context.canceled && rb.linearVelocityY > 0) {
            rb.linearVelocityY *= 0.5f;
            ctElapsed = 0;
        }
    }

    bool IsGrounded() {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    public void EnableControls() {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        playerInput.enabled = true;
    }

    public void DisableControls() {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        playerInput.enabled = false;
    }
}
