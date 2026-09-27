using UnityEngine;

public class objToGet : MonoBehaviour
{
    public string nameOfObj;
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
            for (global::System.Int32 i = 0; i < gameManager.instance.objCoursesToGet.Count; i++)
            {
                if (gameManager.instance.objCoursesToGet[i].Contains(nameOfObj))
                {
                    gameManager.instance.objCoursesToGetTaken[i] = true;
                    Destroy(gameObject);
                }
            }
        }
    }
}
