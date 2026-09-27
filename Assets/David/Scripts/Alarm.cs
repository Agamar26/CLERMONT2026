using UnityEngine;

public class Alarm : MonoBehaviour
{

    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && gameManager.instance.CourseOk)
        {
            audioSource.Play();
        }
    }

}
