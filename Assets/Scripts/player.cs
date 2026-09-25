using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{
    public static player instance;
    public Rigidbody2D rb;
    public Vector2 move;
    public float speed;
    public float radiusDetectGround;
    public LayerMask layerGround;
    public bool isGrounded;
    public float rotY;
    public Camera camera;
    public float cameraSpeed;
    public PlayerInput inputPlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (instance == null)
        {
            instance = this;
        }

        if (Mathf.Round(move.x) != 0)
        {
            if(Mathf.Round(move.x) < 0 )
            {
                rotY = 180;
            }
            else if (Mathf.Round(move.x) > 0)
            {
                rotY = 0;
            }
        }

        transform.rotation = Quaternion.Euler(0, rotY, 0);
        isGrounded = Physics2D.OverlapCircle(transform.position - new Vector3(0, GetComponent<CapsuleCollider2D>().size.y/2, 0), radiusDetectGround, layerGround);

        
    }

    private void FixedUpdate()
    {
        camera.transform.position = Vector3.Lerp(camera.transform.position, new Vector3(transform.position.x, transform.position.y, -10), cameraSpeed * Time.fixedDeltaTime);
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
