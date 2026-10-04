using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerNet : MonoBehaviourPunCallbacks
{
    public Vector2 move;
    public float speed;
    public Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(photonView.IsMine)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            rb.linearVelocity = new Vector2(0, Mathf.Round(move.y) * speed);
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }
}
