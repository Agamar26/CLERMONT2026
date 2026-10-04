using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerNet : MonoBehaviourPunCallbacks
{
    public Vector2 move;
    public float speed;
    public Rigidbody2D rb;
    public int actornumber;
    public TextMeshProUGUI textscore;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        actornumber = GetComponent<PhotonView>().OwnerActorNr;
        if (photonView.IsMine)
        {
            rb = GetComponent<Rigidbody2D>();
            
        } 
        else
        {
            Destroy(this.GetComponent<PlayerInput>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        actornumber = GetComponent<PhotonView>().OwnerActorNr;
        textscore.transform.rotation = Quaternion.Euler(0,0,0);    
        var ezez = PhotonNetwork.CurrentRoom.GetPlayer(actornumber, true).CustomProperties;
        var sdd = ezez.TryGetValue("Score", out object value);
        textscore.text = value.ToString();
      
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
