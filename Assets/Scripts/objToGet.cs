using UnityEngine;

public class objToGet : MonoBehaviour
{
    public string nameOfObj;

    [Header("Son de ramassage")]
    public AudioClip sonRamassage;
    [Range(0f, 1f)] public float volume = 1f;

    [HideInInspector] public int indexCourse = -1;   // rempli par CourseSpawner, -1 = objet hors liste

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (indexCourse < 0 || !collision.CompareTag("Player")) return;

        var gm = gameManager.instance;
        if (gm.objCoursesToGetTaken[indexCourse]) return;

        gm.objCoursesToGetTaken[indexCourse] = true;

        // Joué à la position de la caméra : PlayClipAtPoint crée un son 3D,
        // qui serait atténué s'il partait de l'objet (caméra éloignée en Z)
        if (sonRamassage != null)
            AudioSource.PlayClipAtPoint(sonRamassage, Camera.main.transform.position, volume);

        Destroy(gameObject);
    }
}