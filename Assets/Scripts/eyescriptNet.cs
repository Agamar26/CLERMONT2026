using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using static UnityEngine.UI.Image;

public class eyescriptNet : MonoBehaviourPunCallbacks
{
    public List<Vector2> origin = new List<Vector2>();
    public List<Vector2> direction = new List<Vector2>();
    public float forceLaunch;
    public float timerCanGetBall;
    public LayerMask nulllayermask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient) { return; }

       
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) { return; }

        for (int i = 0; i < origin.Count; i++)
        {
            Debug.DrawRay(origin[i], direction[i], Color.red);
        }
    }
    public void launchBall()
    {
        if (!PhotonNetwork.IsMasterClient) { return; }
        float esds = Random.Range(0, 360);
        GetComponent<Rigidbody2D>().linearVelocity = Quaternion.Euler(0,0, esds) *  Vector2.right * (forceLaunch);
        origin.Add(transform.position);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
       if(PhotonNetwork.IsMasterClient)
        {
            Vector2 contact = collision.contacts[0].point;
            Vector2 eze = Vector2.Reflect((contact - origin[origin.Count - 1]).normalized, collision.contacts[0].normal);
            origin.Add(contact);
            direction.Add(eze);
            GetComponent<Rigidbody2D>().linearVelocity = eze.normalized * (forceLaunch *2);
            
        }
    
    }
}
