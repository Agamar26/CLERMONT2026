using UnityEngine;

// S'exécute avant FlecheObjectif : la cible est mise à jour avant que la flèche la lise
[DefaultExecutionOrder(-100)]
public class GuideCourses : MonoBehaviour
{
    public FlecheObjectif fleche;         // flèche dédiée (pas celle de la sortie ni de l'œil)

    private float timer;

    void Start()
    {
        fleche.Desactiver();
    }

    public void Activer(float duree)
    {
        timer = Mathf.Max(timer, duree);  // reprendre un bonus prolonge, ne raccourcit jamais
        if (fleche.joueur == null) fleche.joueur = player.instance.transform;
        fleche.cible = null;              // force la recherche du prochain article
    }

    void Update()
    {
        if (timer <= 0f) return;
        timer -= Time.deltaTime;

        // Article ramassé (détruit) ou pas encore cherché : on vise le suivant
        if (fleche.cible == null) fleche.cible = ProchainObjet();

        if (timer > 0f && fleche.cible != null)
        {
            fleche.Activer();
        }
        else
        {
            fleche.Desactiver();
            timer = 0f;
        }
    }

    // Premier article de la liste pas encore ramassé
    private Transform ProchainObjet()
    {
        var gm = gameManager.instance;
        var objets = FindObjectsByType<objToGet>(FindObjectsSortMode.None);

        for (int i = 0; i < gm.objCoursesToGetTaken.Count; i++)
        {
            if (gm.objCoursesToGetTaken[i]) continue;
            foreach (var o in objets)
                if (o.indexCourse == i) return o.transform;
        }
        return null;
    }
}