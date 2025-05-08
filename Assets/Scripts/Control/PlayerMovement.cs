using System.Collections;
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

    [Header("Vaulting & Hanging")]
    [SerializeField] Transform headRayPos;
    [SerializeField] Transform bodyRayPos;
    [SerializeField] float rayDistance;
    float rayDirection;
    bool isHanging;
    bool isDropping;
    [SerializeField] float maxHangVelocity;
    [SerializeField] float vaultPower;
    RaycastHit2D headRay;
    RaycastHit2D bodyRay;

    [Header("References")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    PlayerInput playerInput;
    Rigidbody2D rb;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        EnableControls();
    }

    void Update()
    {
        targetSpeed = horizontal * (isRunning ? runSpeed : walkSpeed);
        jbElapsed -= Time.deltaTime;

        JumpingPhysics();
        Hang();
    }

    void JumpingPhysics()
    {
        if (IsGrounded())
        {
            ctElapsed = coyoteTime;
            apexElapsed = 0;
        }
        else
        {
            ctElapsed -= Time.deltaTime;
        }

        if (jbElapsed > 0 && ctElapsed > 0)
        {
            rb.linearVelocityY = jumpPower;
            jbElapsed = 0;
        }

        if (rb.linearVelocityY > 0)
        {
            rb.gravityScale = gravity;
        }
        else if (!IsGrounded())
        {
            if (apexElapsed < apexHang)
            {
                rb.linearVelocityY = 0;
                rb.gravityScale = 0;
                apexElapsed += Time.deltaTime;
            }
            else
            {
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
            rayDirection = dir;
        }
    }

    public void Sprint(InputAction.CallbackContext context) {
        if (context.performed) isRunning = true;
        if (context.canceled) isRunning = false;
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

    public void Vault(InputAction.CallbackContext context) {
        if (context.performed && isHanging) {
            isHanging = false;
            StartCoroutine(DropTimer());
            rb.linearVelocityY = vaultPower;
        }
    }

    public void Drop(InputAction.CallbackContext context) {
        if (context.performed && isHanging) {
            isHanging = false;
            StartCoroutine(DropTimer());
            rb.gravityScale = gravity;
            rb.linearVelocityY = -jumpPower * 0.5f;
        }
    }

    IEnumerator DropTimer() {
        isDropping = true;
        yield return new WaitForSeconds(0.3f);
        isDropping = false;
    }

    void Hang() {
        if (isHanging) {
            rb.gravityScale = 0;
            rb.linearVelocity = Vector2.zero;

            playerInput.actions.FindAction("Move").Disable();
            playerInput.actions.FindAction("Jump").Disable();
            playerInput.actions.FindAction("Vault").Enable();
            playerInput.actions.FindAction("Drop").Enable();
        }
        else {
            rb.gravityScale = gravity;
            if (!isDropping) HangCheck();

            playerInput.actions.FindAction("Move").Enable();
            playerInput.actions.FindAction("Jump").Enable();
            playerInput.actions.FindAction("Vault").Disable();
            playerInput.actions.FindAction("Drop").Disable();
        }
    }

    void HangCheck() {
        headRay = Physics2D.Raycast(headRayPos.position, Vector2.right * rayDirection, rayDistance, groundLayer);
        bodyRay = Physics2D.Raycast(bodyRayPos.position, Vector2.right * rayDirection, rayDistance, groundLayer);

        if (headRay.collider == null && bodyRay.collider != null && rb.linearVelocityY < maxHangVelocity) {
            isHanging = IsGrounded() ? false : true;
        }
        else {
            isHanging = false;
        }
    }

    bool IsGrounded() {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    public void EnableControls() {
        playerInput.enabled = true;

        playerInput.actions.FindAction("Move").Enable();
        playerInput.actions.FindAction("Jump").Enable();
        playerInput.actions.FindAction("Vault").Disable();
        playerInput.actions.FindAction("Drop").Disable();
    }

    public void DisableControls() {
        playerInput.enabled = false;
    }
}
