using UnityEngine;

public class objToGet : MonoBehaviour
{
    public string nameOfObj;

    [HideInInspector] public int indexCourse = -1;   // rempli par CourseSpawner, -1 = objet hors liste

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (indexCourse < 0 || !collision.CompareTag("Player")) return;

        var gm = gameManager.instance;
        if (gm.objCoursesToGetTaken[indexCourse]) return;

        gm.objCoursesToGetTaken[indexCourse] = true;
        Destroy(gameObject);
    }
}