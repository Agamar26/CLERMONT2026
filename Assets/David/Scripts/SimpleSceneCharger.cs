using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleSceneCharger : MonoBehaviour
{
    public string sceneName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Fondu.Charger(sceneName);
        }
    }
}
