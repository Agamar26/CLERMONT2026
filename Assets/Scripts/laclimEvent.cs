using JetBrains.Annotations;
using UnityEngine;

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
}
