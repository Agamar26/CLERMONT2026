using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.UIElements;
using UnityEngine;

public class eyeScript : MonoBehaviour
{
    public bool launched;
    public List<Vector2> origin = new List<Vector2>();
    public List<Vector2> direction = new List<Vector2>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         for (int i = 0; i < origin.Count; i++)
        {
            Debug.DrawRay(origin[i], direction[i], Color.red);
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
            GetComponent<Rigidbody2D>().linearVelocity = eze * player.instance.forceLaunch;
        }
    }

   
}
