using UnityEngine;
using UnityEngine.Audio;

public class MusiActivation : MonoBehaviour
{
    private AudioSource audioSource;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Vérifie si l'AudioSource est assignée
            if (audioSource != null)
            {
                audioSource.enabled = true;
            }
            else
            {
                Debug.LogWarning("AudioSource n'est pas assignée sur MusiActivation.");
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Vérifie si l'AudioSource est assignée
            if (audioSource != null)
            {
                audioSource.enabled = false;
            }
            else
            {
                Debug.LogWarning("AudioSource n'est pas assignée sur MusiActivation.");
            }
        }
    }
}
