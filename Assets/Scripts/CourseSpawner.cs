using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CourseSpawner : MonoBehaviour
{
    public objToGet[] prefabs;                 // tous les objets possibles
    public Transform[] spawnPoints;
    public bool remplirPointsLibres = true;    // place les objets hors liste sur les points restants

    IEnumerator Start()
    {
        // Attend que le gameManager ait rempli la liste de courses
        while (gameManager.instance == null ||
               gameManager.instance.objCoursesToGet.Count < gameManager.instance.numberOfTasks)
            yield return null;

        var aPlacer = gameManager.instance.objCoursesToGet;

        if (spawnPoints.Length < aPlacer.Count)
            Debug.LogError($"Pas assez de points de spawn : {spawnPoints.Length} pour {aPlacer.Count} objets.");

        var points = Melanger(new List<Transform>(spawnPoints));
        int p = 0;

        // 1. Les objets de la liste de courses, en priorité
        foreach (var texte in aPlacer)
        {
            if (p >= points.Count) break;

            var prefab = Trouver(texte);
            if (prefab == null)
            {
                Debug.LogError($"Aucun prefab dont le nameOfObj est contenu dans \"{texte}\".");
                continue;
            }

            Instantiate(prefab, points[p++].position, Quaternion.identity);
        }

        if (!remplirPointsLibres) yield break;

        // 2. Les autres objets sur les points restants
        var autres = new List<objToGet>();
        foreach (var o in prefabs)
            if (!SurLaListe(o, aPlacer)) autres.Add(o);

        if (autres.Count == 0) yield break;

        var pioche = new List<objToGet>();
        while (p < points.Count)
        {
            if (pioche.Count == 0) pioche = Melanger(new List<objToGet>(autres));

            Instantiate(pioche[pioche.Count - 1], points[p++].position, Quaternion.identity);
            pioche.RemoveAt(pioche.Count - 1);
        }
    }

    // Même règle que objToGet : le texte de la liste contient le nameOfObj
    private objToGet Trouver(string texte)
    {
        foreach (var o in prefabs)
            if (texte.Contains(o.nameOfObj)) return o;
        return null;
    }

    private bool SurLaListe(objToGet o, List<string> liste)
    {
        foreach (var texte in liste)
            if (texte.Contains(o.nameOfObj)) return true;
        return false;
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