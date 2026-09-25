using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector2 move;
    public float speed;
    public float radiusDetectGround;
    public LayerMask layerGround;
    public bool isGrounded;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(transform.position - new Vector3(0, GetComponent<CapsuleCollider2D>().size.y/2, 0), radiusDetectGround, layerGround);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(Mathf.Round(move.x) * speed, rb.linearVelocityY);
    }



    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position - new Vector3(0, GetComponent<CapsuleCollider2D>().size.y/2, 0), radiusDetectGround);
    }

}
