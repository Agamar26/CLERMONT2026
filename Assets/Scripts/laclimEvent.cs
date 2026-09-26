using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class laclimEvent : MonoBehaviour
{
    public GameObject balleye;
    public Transform pointbras;
    public GameObject eyeobjplayer;
    public GameObject eyetolaunch;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void takeBallEye()
    {
        
        gameManager.instance.animatorSpeed = 0.5f;
        eyeobjplayer.GetComponent<SpriteRenderer>().enabled = false;
        eyetolaunch = Instantiate(balleye,pointbras.transform);
    }
  
    public void bringbackBallEye()
    {
       
        gameManager.instance.animatorSpeed = 1f;
        Destroy(eyetolaunch);
        eyeobjplayer.GetComponent<SpriteRenderer>().enabled = true;
        
    }

    public void launchBallEye()
    {
        player.instance.donthaveEye = true;
        var eez = pointbras.transform.position;
        eyetolaunch.transform.parent = null;
        eyetolaunch.transform.position = transform.TransformPoint(pointbras.localPosition); 
        eyetolaunch.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        eyetolaunch.GetComponent<Rigidbody2D>().linearVelocity = player.instance.launchdirection * player.instance.forceLaunch;
        eyetolaunch.GetComponent<eyeScript>().launched = true;
        eyetolaunch.GetComponent<eyeScript>().origin.Add(eyetolaunch.transform.position);
        eyetolaunch.GetComponent<eyeScript>().direction.Add(player.instance.launchdirection.normalized * player.instance.forceLaunch);
        player.instance.state = player.playerstate.launch;

    }
}
