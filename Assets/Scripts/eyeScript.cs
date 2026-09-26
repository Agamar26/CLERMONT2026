using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.UIElements;
using UnityEngine;

public class eyeScript : MonoBehaviour
{
    public bool launched;
    public List<Vector2> origin = new List<Vector2>();
    public List<Vector2> direction = new List<Vector2>();
    public float divise;
    public float timerCanGetBall;
    public LayerMask nulllayermask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        divise = 1;
    }

    // Update is called once per frame
    void Update()
    {
         for (int i = 0; i < origin.Count; i++)
        {
            Debug.DrawRay(origin[i], direction[i], Color.red);
        }
         if(!launched ) {return;}
        if(timerCanGetBall < 0.5f)
        {
            timerCanGetBall += 1f * Time.deltaTime;
        }
        else
        {
            if (Vector2.Distance(transform.position, player.instance.transform.position) < 1)
            {
                // Utilise Vector3 pour les angles euler
                transform.localEulerAngles = Vector3.MoveTowards(transform.localEulerAngles, player.instance.eyePos.localEulerAngles, 20 * Time.deltaTime);
                transform.position = Vector2.MoveTowards(transform.position, player.instance.eyePos.position, 20 * Time.deltaTime);

                // On vérifie si on est très proche de eyePos.position (avec un seuil au lieu de ==)
                if (Vector2.Distance(transform.position, player.instance.eyePos.position) < 0.05f)
                {
                    player.instance.eyePos.GetComponent<SpriteRenderer>().enabled = true;
                    player.instance.donthaveEye = false;

                    if (player.instance.state == player.playerstate.confused)
                    {
                        player.instance.state = player.playerstate.idle;
                    }

                    Destroy(gameObject);

                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!launched) { return; }
        if(collision.gameObject.tag == "Ground")
        {
            
            var ezez = Physics2D.Raycast(origin[origin.Count-1],(Vector2)transform.position- origin[origin.Count-1], 50);
            
            
            var eze = Vector2.Reflect((collision.contacts[0].point - origin[origin.Count - 1]).normalized, collision.contacts[0].normal);
            origin.Add(collision.contacts[0].point);
            direction.Add(eze);
            GetComponent<Rigidbody2D>().linearVelocity = eze.normalized * (player.instance.forceLaunch/ divise);
            divise += 0.5f;
        }
    }

   
}
