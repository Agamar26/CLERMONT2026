using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;

using UnityEngine;

public class eyescriptNet : MonoBehaviourPunCallbacks
{
    public List<Vector2> origin = new List<Vector2>();
    public List<Vector2> direction = new List<Vector2>();
    public float forceLaunch;
    public float timerCanGetBall;
    public LayerMask nulllayermask;
    public int plusball;
    public int actornumber;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient) { return; }

       
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) { return; }

   
      
    }
    public void launchBall()
    {
        if (!PhotonNetwork.IsMasterClient) { return; }
        float esds = Random.Range(0, 360);
        GetComponent<Rigidbody2D>().linearVelocity = Quaternion.Euler(0,0, esds) *  Vector2.right * (forceLaunch);
        origin.Add(transform.position);
    }

    [PunRPC]
    public void PlayingSound(string tagname,string leftorright)
    {
        if (tagname == "Player")
        {
            networkManager.instance.audioyar.Play();
        }
        var ezez = GameObject.FindGameObjectsWithTag("Player").ToList();
        var ezeqssqsq = networkManager.instance.collidergauche;
        if(leftorright == "left")
        {
            ezeqssqsq = networkManager.instance.collidergauche;
        }
        else
        {
            ezeqssqsq = networkManager.instance.colliderdroite;

        }
            GameObject joueurDroite = ezez
                                .OrderBy(a => Vector2.Distance(
                                    a.transform.position,
                                    ezeqssqsq.gameObject.transform.position
                                ))
                                .LastOrDefault();
        if (tagname == "Death")
        {

            if (joueurDroite.GetComponent<playerNet>().actornumber != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                networkManager.instance.no.Play();
                foreach (var item in ezez)
                {
                    if (item.GetComponent<playerNet>().actornumber != PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        item.GetComponentInChildren<Animator>().SetTrigger("Scored");
                    }
                }
            }
            else
            {
                networkManager.instance.yes.Play();
                foreach (var item in ezez)
                {
                    if (item.GetComponent<playerNet>().actornumber == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        item.GetComponentInChildren<Animator>().SetTrigger("Scored");
                    }
                }
            }


        }

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
                photonView.RPC("PlayingSound", RpcTarget.AllViaServer, "Player","left");
            }
            if (collision.gameObject.tag == "Death")
            {
                if (collision.gameObject == networkManager.instance.colliderdroite)
                {
                    photonView.RPC("PlayingSound", RpcTarget.AllViaServer, "Death", "Right");
                }
                else
                {
                    photonView.RPC("PlayingSound", RpcTarget.AllViaServer, "Death", "Left");
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
