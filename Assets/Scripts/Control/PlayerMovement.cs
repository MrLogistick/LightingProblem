using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float gravity;
    [SerializeField] float groundCheckDistance;
    [SerializeField] LayerMask groundLayer;

    void Update() {
        print(IsGrounded());
        Debug.DrawRay(transform.position, -transform.up * groundCheckDistance, Color.red);
    }
    
    bool IsGrounded() {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, groundCheckDistance, groundLayer);
        return hit.collider != null;
    }
}
