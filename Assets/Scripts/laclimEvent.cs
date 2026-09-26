using UnityEngine;

public class laclimEvent : MonoBehaviour
{
    public GameObject balleye;
    public Transform pointbras;
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
       var ee = Instantiate(balleye,pointbras.transform.position,Quaternion.identity);
    }
}
