using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;
using UnityEngine.WSA;
using static enemy;

public class player : MonoBehaviour
{
    public static player instance;
    public Rigidbody2D rb;
    public Vector2 move;
    public Vector2 look;
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
    public enum playerstate { idle,stuck,frost,confused , prelaunch,launch };
    public playerstate state;
    public bool cantMove;
    public GameObject particlesFrost;
    public Transform brasPoint;
    public float rotbras;
    public Vector2 launchdirection;
    public float forceLaunch;
    public bool donthaveEye;
    public Transform eyePos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale =1f;
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
                var ezze = camera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                var ez = Mathf.Atan2(ezze.y, ezze.x) * Mathf.Rad2Deg;
                brasPoint.transform.rotation = Quaternion.Euler(0, 0, ez);
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
            case playerstate.confused:
                if (Mathf.Round(move.x) != 0 || Mathf.Round(move.y) != 0)
                {
                    animatorClim.SetBool("Run", true);
                    if (Mathf.Round(move.x) < 0)
                    {
                        rotY = 0;
                    }
                    else if (Mathf.Round(move.x) > 0)
                    {
                        rotY = 180;
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
            case playerstate.prelaunch:
                
                
                if (inputPlayer.currentControlScheme == "Gamepad")
                {
                    launchdirection = (look).normalized;
                }
                else
                {
                    launchdirection = (look - (Vector2)transform.position).normalized;
                }


                var ezqq = Mathf.Atan2(launchdirection.y, launchdirection.x) * Mathf.Rad2Deg;
                print(ezqq);
               
                brasPoint.transform.rotation = Quaternion.Euler(0,0, ezqq);
                if (inputPlayer.currentControlScheme == "Gamepad")
                {
                    if (look.x > 0)
                    {
                        rotY = 0;
                    }
                    else if (look.x < 0)
                    {
                        rotY = 180;
                    }
                }
                else
                {
                    if ( look.x > transform.position.x)
                    {
                        rotY = 0;
                    }
                    else if (look.x < transform.position.x)
                    {
                        rotY = 180;
                    }
                }
               
                
               
                transform.rotation = Quaternion.Euler(0, rotY, 0);
                break;
            case playerstate.launch:
              
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
        else if(state == playerstate.confused)
        {
            rb.linearVelocity = new Vector2(-Mathf.Round(move.x) * speed, -Mathf.Round(move.y) * speed);
        }
        else if(state == playerstate.prelaunch || state == playerstate.launch)
        {
            rb.linearVelocity = new Vector2(0, 0);
        }
        
    }
    public void AttackDebut(InputAction.CallbackContext context)
    {
        if (state == playerstate.prelaunch)
        {
            animatorClim.SetTrigger("Launch");
            
        }
        else
        {
            animatorClim.SetBool("Frost", true);
        }
    }
    public void AttackFin(InputAction.CallbackContext context)
    {
        if (state == playerstate.prelaunch)
        {

        }
        else
        {
            animatorClim.SetBool("Frost", false);
        }
       

    }
    public void preLaunchDebut(InputAction.CallbackContext context)
    {
        if(state == playerstate.confused ||state == playerstate.prelaunch ||state == playerstate.launch || donthaveEye) { return; }
        animatorClim.SetBool("Prelaunch", true);
    }
    public void preLaunchFin(InputAction.CallbackContext context)
    {
        if (state == playerstate.confused) { return; }
        animatorClim.SetBool("Prelaunch", false);

    }
    private void OnEnable()
    {
        inputPlayer.actions.FindAction("Attack").started += AttackDebut;
        inputPlayer.actions.FindAction("Attack").canceled += AttackFin;
        inputPlayer.actions.FindAction("Jump").started += preLaunchDebut;
        inputPlayer.actions.FindAction("Jump").canceled += preLaunchFin;
    }

    private void OnDisable()
    {
        inputPlayer.actions.FindAction("Attack").started -= AttackDebut;
        inputPlayer.actions.FindAction("Attack").canceled -= AttackFin;
        inputPlayer.actions.FindAction("Jump").started -= preLaunchDebut;
        inputPlayer.actions.FindAction("Jump").canceled -= preLaunchFin;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }
    public void Look(InputAction.CallbackContext context)
    {
        if(inputPlayer.currentControlScheme == "Gamepad")
        {
            look = context.ReadValue<Vector2>();
        }
        else
        {
            look = camera.ScreenToWorldPoint(context.ReadValue<Vector2>());
        }
       
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position - new Vector3(0, GetComponent<CapsuleCollider2D>().size.y/2, 0), radiusDetectGround);
    }

}
