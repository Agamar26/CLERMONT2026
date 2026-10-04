using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;

using UnityEngine;
using UnityEngine.UIElements;

public class eyescriptNet : MonoBehaviourPunCallbacks
{
    public List<Vector2> origin = new List<Vector2>();
    public List<Vector2> direction = new List<Vector2>();
    public float forceLaunch;
    public float timerCanGetBall;
    public LayerMask nulllayermask;
    public int plusball;
    public int actornumber;
    public float scale = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient) { return; }
        scale = transform.localScale.x;


    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) { return; }

   
      
    }
    public void launchBall()
    {
        if (!PhotonNetwork.IsMasterClient) { return; }
        int random = Random.Range(0, 2);
        float esds = 5;
        if (random == 0)
        {
             esds = Random.Range(-50,50);
        }
        else
        {
            esds = Random.Range(-130, -230);
        }

        GetComponent<Rigidbody2D>().linearVelocity = Quaternion.Euler(0,0, esds) *  Vector2.right * (forceLaunch);
        origin.Add(transform.position);
    }

   

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var ezez = GameObject.FindGameObjectsWithTag("Player").ToList();

        GameObject joueurDroite = ezez
                            .OrderBy(a => Vector2.Distance(
                                a.transform.position,
                                collision.gameObject.transform.position
                            ))
                            .LastOrDefault();
        
      
       if(PhotonNetwork.IsMasterClient)
        {
            if (collision.gameObject.tag == "Player")
            {
                scale -= 0.2f;
                transform.localScale = new Vector3(scale, scale, scale);
                networkManager.instance.photonView.RPC(
    "PlayingSound",
    RpcTarget.AllViaServer,
    "Player",
    "Left"
);
            }
            if (collision.gameObject.tag == "Death")
            {
                if (collision.gameObject == networkManager.instance.colliderdroite)
                {
                                    networkManager.instance.photonView.RPC(
                    "PlayingSound",
                    RpcTarget.AllViaServer,
                    "Death",
                    "right"
                );
                }
                else
                {
                    networkManager.instance.photonView.RPC(
    "PlayingSound",
    RpcTarget.AllViaServer,
    "Death",
    "left"
);
                }


            }

            if (collision.gameObject.tag != "Death")
            {
                if (collision.gameObject.tag == "Player")
                {
                    actornumber = collision.gameObject.GetComponent<PhotonView>().OwnerActorNr;
                    plusball++;
                    Vector2 contact = collision.contacts[0].point;
                    Vector2 eze = Vector2.Reflect((contact - origin[origin.Count - 1]).normalized, collision.contacts[0].normal);
                    origin.Add(contact);
                    direction.Add(collision.contacts[0].normal);
                    GetComponent<Rigidbody2D>().linearVelocity = collision.contacts[0].normal * (forceLaunch + plusball);
                }
                else
                {
                    Vector2 contact = collision.contacts[0].point;
                    Vector2 eze = Vector2.Reflect((contact - origin[origin.Count - 1]).normalized, collision.contacts[0].normal);
                    origin.Add(contact);
                    direction.Add(eze);
                    GetComponent<Rigidbody2D>().linearVelocity = eze.normalized * (forceLaunch + plusball);
                }
                
               
            }
            if (collision.gameObject.tag == "Death")
            {
              
                    var ezefffz = PhotonNetwork.CurrentRoom.GetPlayer(joueurDroite.GetComponent<playerNet>().actornumber, true).CustomProperties;
                    var sdd = ezefffz.TryGetValue("Score", out object value);
                    int ezedddd = int.Parse((string)value);
                    var inta = ezedddd += 1 ;
                    ezefffz["Score"] = inta.ToString();
                     //ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
                    //hash.Add("Score", inta.ToString());
                    PhotonNetwork.CurrentRoom.GetPlayer(joueurDroite.GetComponent<playerNet>().actornumber, true).SetCustomProperties(ezefffz);
                
                



                networkManager.instance.currentstateName = "respawnball";
                networkManager.instance.state = networkManager.stateGame.respawnball;
                PhotonNetwork.Destroy(this.gameObject);
              
            }
            
        }
    
    }
}
