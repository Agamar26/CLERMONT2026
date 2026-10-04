using UnityEngine;

public class BonusGuide : MonoBehaviour
{
    public float duree = 8f;

    [Header("Son de ramassage")]
    public AudioClip son;
    [Range(0f, 1f)] public float volume = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        var guide = FindFirstObjectByType<GuideCourses>();
        if (guide == null)
        {
            Debug.LogWarning("Aucun GuideCourses dans la scène.");
            return;
        }

        guide.Activer(duree);

        if (son != null && player.instance != null && player.instance.audioActions != null)
            player.instance.audioActions.PlayOneShot(son, volume);

        Destroy(gameObject);
    }
}