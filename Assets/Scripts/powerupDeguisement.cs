using UnityEngine;

public class powerupDeguisement : MonoBehaviour
{
    private float durée = 3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            var ezez = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var item in ezez)
            {
                item.GetComponent<EnemyStats>().ModifySpeed(0, durée);
            }

           player.instance.Deguiser(durée);
            Destroy(gameObject);
        }
    }
}
