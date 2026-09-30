using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CourseSpawner : MonoBehaviour
{
    public objToGet[] prefabs;                 // tous les objets possibles
    public Transform[] spawnPoints;
    public bool remplirPointsLibres = true;    // place les objets hors liste sur les points restants
    public float distanceMiniJoueur = 2f;      // les objets de la liste évitent les points trop proches du joueur

    IEnumerator Start()
    {
        // Attend que le gameManager ait rempli la liste de courses
        while (gameManager.instance == null ||
               gameManager.instance.objCoursesToGet.Count < gameManager.instance.numberOfTasks)
            yield return null;

        var aPlacer = gameManager.instance.objCoursesToGet;

        if (spawnPoints.Length < aPlacer.Count)
            Debug.LogError($"Pas assez de points de spawn : {spawnPoints.Length} pour {aPlacer.Count} objets.");

        VerifierPoints();

        var points = Melanger(new List<Transform>(spawnPoints));

        // Points proches du joueur en dernier : un objet posé sur lui serait ramassé instantanément
        if (player.instance != null)
        {
            Vector2 pj = player.instance.transform.position;
            var loin = points.FindAll(t => t != null && Vector2.Distance(t.position, pj) >= distanceMiniJoueur);
            var proche = points.FindAll(t => t != null && Vector2.Distance(t.position, pj) < distanceMiniJoueur);
            loin.AddRange(proche);
            points = loin;
        }

        int p = 0;
        var surLaListe = new HashSet<objToGet>();

        // 1. Les objets de la liste de courses, en priorité
        for (int i = 0; i < aPlacer.Count; i++)
        {
            var prefab = Trouver(aPlacer[i]);
            if (prefab == null)
            {
                Debug.LogError($"Aucun prefab trouvé pour \"{aPlacer[i]}\" : vérifie le nameOfObj.");
                continue;
            }

            if (p >= points.Count)
            {
                Debug.LogError($"Plus de point de spawn libre : \"{aPlacer[i]}\" n'est pas placé.");
                continue;
            }

            surLaListe.Add(prefab);
            var obj = Instantiate(prefab, points[p++].position, Quaternion.identity);
            obj.indexCourse = i;
        }

        if (!remplirPointsLibres) yield break;

        // 2. Les autres objets sur les points restants (indexCourse = -1, non ramassables)
        var autres = new List<objToGet>();
        foreach (var o in prefabs)
            if (o != null && !surLaListe.Contains(o)) autres.Add(o);

        if (autres.Count == 0) yield break;

        var pioche = new List<objToGet>();
        while (p < points.Count)
        {
            if (pioche.Count == 0) pioche = Melanger(new List<objToGet>(autres));

            Instantiate(pioche[pioche.Count - 1], points[p++].position, Quaternion.identity);
            pioche.RemoveAt(pioche.Count - 1);
        }
    }

    // Égalité exacte d'abord, sinon le nameOfObj le plus long contenu dans le texte
    private objToGet Trouver(string texte)
    {
        if (string.IsNullOrEmpty(texte)) return null;
        string t = texte.Trim();

        foreach (var o in prefabs)
        {
            if (o == null || string.IsNullOrWhiteSpace(o.nameOfObj)) continue;
            if (string.Equals(o.nameOfObj.Trim(), t, System.StringComparison.OrdinalIgnoreCase)) return o;
        }

        objToGet meilleur = null;
        foreach (var o in prefabs)
        {
            if (o == null || string.IsNullOrWhiteSpace(o.nameOfObj)) continue;
            string n = o.nameOfObj.Trim();
            if (t.IndexOf(n, System.StringComparison.OrdinalIgnoreCase) < 0) continue;
            if (meilleur == null || n.Length > meilleur.nameOfObj.Trim().Length) meilleur = o;
        }
        return meilleur;
    }

    // Prévient si deux points de spawn sont au même endroit (un objet en cacherait un autre)
    private void VerifierPoints()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] == null)
            {
                Debug.LogWarning($"Le spawn point {i} est vide.");
                continue;
            }
            for (int j = i + 1; j < spawnPoints.Length; j++)
            {
                if (spawnPoints[j] != null &&
                    Vector3.Distance(spawnPoints[i].position, spawnPoints[j].position) < 0.1f)
                    Debug.LogWarning($"Les spawn points {i} et {j} sont superposés.");
            }
        }
    }

    private List<T> Melanger<T>(List<T> liste)
    {
        for (int i = liste.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (liste[i], liste[j]) = (liste[j], liste[i]);
        }
        return liste;
    }
}