using UnityEngine;

public class objToGet : MonoBehaviour
{
    public string nameOfObj;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        var gm = gameManager.instance;
        int index = gm.objCoursesToGet.IndexOf(nameOfObj);
        if (index < 0) return;   // pas sur la liste : l'objet reste en rayon

        gm.objCoursesToGetTaken[index] = true;
        Destroy(gameObject);
    }
}