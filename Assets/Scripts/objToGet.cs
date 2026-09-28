using UnityEngine;

public class objToGet : MonoBehaviour
{
    public string nameOfObj;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        var gm = gameManager.instance;
        for (int i = 0; i < gm.objCoursesToGet.Count; i++)
        {
            if (!gm.objCoursesToGetTaken[i] && gm.objCoursesToGet[i].Contains(nameOfObj))
            {
                gm.objCoursesToGetTaken[i] = true;
                Destroy(gameObject);
                return;
            }
        }
    }
}