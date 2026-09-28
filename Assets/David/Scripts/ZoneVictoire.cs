using UnityEngine;

public class ZoneVictoire : MonoBehaviour
{
    public string sceneVictoire = "MiniJeu";

    private bool declenche;

    // Stay plutôt qu'Enter : fonctionne même si le joueur est déjà dans la zone
    // au moment où il ramasse le dernier objet
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (declenche || !collision.CompareTag("Player")) return;
        if (!gameManager.instance.CourseOk) return;

        declenche = true;
        Fondu.Charger(sceneVictoire);
    }
}