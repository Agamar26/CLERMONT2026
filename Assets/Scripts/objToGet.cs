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

        // Son 2D joué sur la source du joueur : pas d'atténuation, et il survit au Destroy
        if (sonRamassage != null && player.instance != null && player.instance.audioActions != null)
            player.instance.audioActions.PlayOneShot(sonRamassage, volume);

        Destroy(gameObject);
    }
}