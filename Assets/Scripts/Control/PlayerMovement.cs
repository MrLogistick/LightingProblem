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

    [Header("Vaulting")]
    [SerializeField] Vector2 topCastPosition;
    [SerializeField] Vector2 centreCastPosition;
    [SerializeField] float rayDistance;
    [SerializeField] Vector2 vaultEndPosition;

    [Header("References")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    Rigidbody2D rb;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        targetSpeed = horizontal * (isRunning ? runSpeed : walkSpeed);
        jbElapsed -= Time.deltaTime;

        VaultCheck();

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

        if (horizontal < 0) {
            transform.localScale = new Vector3(-1, 1.8f, 1);
        }
        if (horizontal > 0) {
            transform.localScale = new Vector3(1, 1.8f, 1);
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

    void VaultCheck() {
        RaycastHit2D topHit = Physics2D.Raycast(topCastPosition, transform.right, rayDistance, groundLayer);
        RaycastHit2D centerHit = Physics2D.Raycast(centreCastPosition, transform.right, rayDistance, groundLayer);

        // Debug lines to visualize the raycasts
        Debug.DrawLine(topCastPosition, topCastPosition + (Vector2)transform.right * rayDistance, Color.green);
        Debug.DrawLine(centreCastPosition, centreCastPosition + (Vector2)transform.right * rayDistance, Color.green);

        // Vault condition: top is clear, center is blocked
        if (topHit.collider == null && centerHit.collider != null)
        {
            Vector2 currentPos = transform.position;
            transform.position = currentPos + vaultEndPosition; // vaultEndPosition should be relative (offset)
        }
    }

    bool IsGrounded() {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
}
