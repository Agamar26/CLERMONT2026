using System.Collections.Generic;
using System.Linq;
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
    public Vector2 elevatormovedebut;
    public Vector2 elevatormoveup;
    public enum state { waitformount,pause,mount};
    public state stateascenceur;
    public List<Collider2D> colliders = new List<Collider2D>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        elevatormovedebut = transform.position;
        elevatormoveup = (Vector2)transform.position + new Vector2(0, differenceBeetwenEtage);

        elevatormove = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        switch(stateascenceur)
        {
            case state.waitformount:
                if (Vector2.Distance(elevatormovedebut, player.instance.transform.position) > Vector2.Distance(elevatormoveup, player.instance.transform.position))
                {
                    transform.position = elevatormoveup;
                    currentEtage = 0;
                }
                else
                {
                    transform.position = elevatormovedebut;
                    currentEtage = 1;
                }

                if (!changeEtage && playerCanUse)
                {
                    if (Mathf.Round(player.instance.move.y) != 0)
                    {
                        if (Mathf.Round(player.instance.move.y) > 0 && (Vector2)transform.position == elevatormovedebut)
                        {
                            elevatormove = elevatormoveup;
                            currentEtage = 1;
                            pauseTimer = 0;
                            stateascenceur = state.mount;
                        }
                        else if (Mathf.Round(player.instance.move.y) < 0 && (Vector2)transform.position == elevatormoveup)
                        {
                            elevatormove = elevatormovedebut;
                            currentEtage = 0;
                            pauseTimer = 0;
                            stateascenceur = state.mount;
                        }
                    }
                }
                break;

            case state.pause:
                foreach (var item in colliders)
                {
                    item.gameObject.SetActive(false);
                }
                if (pauseTimer != pauseTime)
                {
                    pauseTimer = Mathf.MoveTowards(pauseTimer, pauseTime, 1f * Time.deltaTime);
                }else
                {
                    stateascenceur = state.waitformount;
                }

                break;

            case state.mount:
                foreach (var item in colliders)
                {
                    item.gameObject.SetActive(true);
                }
                if ((Vector2)transform.position != elevatormove)
                {
                    transform.position = Vector2.MoveTowards((Vector2)transform.position,elevatormove,speedelevator * Time.deltaTime);
                }
                else
                {
                    stateascenceur = state.pause;
                }

                break;
        }
       


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.transform.parent = transform;
            print("Can use elevator");
            playerCanUse = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.transform.parent = null;
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
