using UnityEngine;
using UnityEngine.InputSystem;

public class ascenceur : MonoBehaviour
{
    public int currentEtage;
    public float speedelevator;
    public float pauseTimer,pauseTime;
    public bool playerCanUse;
    public bool changeEtage;
    public float differenceBeetwenEtage;
    public Vector2 elevatormove;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        elevatormove = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(!changeEtage && playerCanUse)
        {
            if(Mathf.Round(player.instance.move.y) != 0)
            {
                if(Mathf.Round(player.instance.move.y) > 0 && currentEtage == 0)
                {
                    elevatormove.y += differenceBeetwenEtage;
                }
                else if(Mathf.Round(player.instance.move.y) < 0 && currentEtage == 1)
                {
                    elevatormove.y -= differenceBeetwenEtage;
                }
            }
        }


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            print("Can use elevator");
            playerCanUse = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            print("Can't use elevator");
            playerCanUse = false;
        }
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

   
}
