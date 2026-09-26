using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;
using static enemy;

public class player : MonoBehaviour
{
    public static player instance;
    public Rigidbody2D rb;
    public Vector2 move;
    public float speed;
    public float stamina=100f;
    public float maxStamina = 100f;
    public float health = 100f;
    public float maxHealth = 100f;
    public float radiusDetectGround;
    public LayerMask layerGround;
    public bool isGrounded;
    public float rotY;
    public Camera camera;
    public float cameraSpeed;
    public PlayerInput inputPlayer;
    public Animator animatorClim;
    public enum playerstate { idle,stuck,frost };
    public playerstate state;
    public bool cantMove;
    public GameObject particlesFrost;


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

        switch (state)
        {
            case playerstate.idle:
                if (Mathf.Round(move.x) != 0 || Mathf.Round(move.y) != 0)
                {
                    animatorClim.SetBool("Run", true);
                    if (Mathf.Round(move.x) < 0)
                    {
                        rotY = 180;
                    }
                    else if (Mathf.Round(move.x) > 0)
                    {
                        rotY = 0;
                    }
                }
                else
                {
                    animatorClim.SetBool("Run", false);
                }

                transform.rotation = Quaternion.Euler(0, rotY, 0);
                isGrounded = Physics2D.OverlapCircle(transform.position - new Vector3(0, GetComponent<CapsuleCollider2D>().size.y / 2, 0), radiusDetectGround, layerGround);
                break;

            case playerstate.stuck:
                animatorClim.SetBool("Run", false);
                break;
            case playerstate.frost:
                animatorClim.SetBool("Run", false);
                break;

           
        }
     

        

        
    }

    private void FixedUpdate()
    {

        camera.transform.position = Vector3.Lerp(camera.transform.position, new Vector3(transform.position.x, transform.position.y, -10), cameraSpeed * Time.fixedDeltaTime);
        if(state == playerstate.idle)
        {
            rb.linearVelocity = new Vector2(Mathf.Round(move.x) * speed, Mathf.Round(move.y) * speed);
        }
        else if(state == playerstate.stuck)
        {
            rb.linearVelocity = new Vector2(0, 0);
        }
        
    }
    public void AttackDebut(InputAction.CallbackContext context)
    {
        animatorClim.SetBool("Frost", true);
    }
    public void AttackFin(InputAction.CallbackContext context)
    {
        animatorClim.SetBool("Frost", false);

    }
    private void OnEnable()
    {
        inputPlayer.actions.FindAction("Attack").started += AttackDebut;
        inputPlayer.actions.FindAction("Attack").canceled += AttackFin;
    }

    private void OnDisable()
    {
        inputPlayer.actions.FindAction("Attack").started -= AttackDebut;
        inputPlayer.actions.FindAction("Attack").canceled -= AttackFin;
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
