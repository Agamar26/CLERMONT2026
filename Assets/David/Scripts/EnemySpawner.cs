using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public Collider2D limites;
    public float spawnInterval = 5f;
    public int maxEnemies = 20;

    [Header("Groupe")]
    public int tailleMin = 3;
    public int tailleMax = 4;
    public float rayonGroupe = 1.5f;

    [Header("Boss")]
    public GameObject bossPrefab;
    public int premiereVagueBoss = 5;   // vague de première apparition
    public int vaguesApresMort = 3;     // délai en vagues après sa mort
    public bool bossSeul = false;       // true = le boss arrive sans escorte

    [Header("Sécurité")]
    public float distanceMinJoueur = 4f;
    public bool horsEcran = true;
    public int essaisMax = 20;

    private readonly List<GameObject> ennemis = new List<GameObject>();
    private GameObject boss;
    private bool bossVivant;
    private int prochaineVagueBoss;
    private Camera cam;
    private float nextSpawnTime;
    private int vague;
    private bool actif;

    void Start()
    {
        cam = Camera.main;
        prochaineVagueBoss = premiereVagueBoss;
    }

    void Update()
    {
        // Détection de la mort du boss : le délai part de la vague en cours
        if (bossVivant && boss == null)
        {
            bossVivant = false;
            prochaineVagueBoss = vague + vaguesApresMort;
        }

        // Le spawn ne tourne que si le joueur est dans la zone
        bool joueurDedans = player.instance != null &&
                            limites.OverlapPoint(player.instance.transform.position);

        if (!joueurDedans)
        {
            actif = false;
            return;
        }

        // Entrée dans la zone : le chrono démarre maintenant
        if (!actif)
        {
            actif = true;
            nextSpawnTime = Time.time + spawnInterval;
            return;
        }

        if (Time.time < nextSpawnTime) return;
        nextSpawnTime = Time.time + spawnInterval;

        ennemis.RemoveAll(e => e == null);

        if (!TrouverPosition(limites.bounds, out Vector3 centre)) return;

        vague++;

        if (bossPrefab != null && !bossVivant && vague >= prochaineVagueBoss)
        {
            boss = Instantiate(bossPrefab, centre, Quaternion.identity);
            bossVivant = true;
            if (bossSeul) return;
        }

        SpawnGroupe(centre);
    }

    private void SpawnGroupe(Vector3 centre)
    {
        if (enemyPrefabs.Length == 0) return;

        int place = maxEnemies - ennemis.Count;
        int taille = Mathf.Min(Random.Range(tailleMin, tailleMax + 1), place);

        foreach (var prefab in ChoisirTypes(taille))
        {
            Vector3 pos = TrouverPrès(centre, out Vector3 proche) ? proche : centre;
            ennemis.Add(Instantiate(prefab, pos, Quaternion.identity));
        }
    }

    // Mélange les prefabs, puis les répète si le groupe est plus grand que le nombre de types
    private List<GameObject> ChoisirTypes(int taille)
    {
        var resultat = new List<GameObject>();
        var pioche = new List<GameObject>();

        while (resultat.Count < taille)
        {
            if (pioche.Count == 0)
            {
                pioche.AddRange(enemyPrefabs);
                for (int i = pioche.Count - 1; i > 0; i--)
                {
                    int j = Random.Range(0, i + 1);
                    (pioche[i], pioche[j]) = (pioche[j], pioche[i]);
                }
            }

            resultat.Add(pioche[pioche.Count - 1]);
            pioche.RemoveAt(pioche.Count - 1);
        }

        return resultat;
    }

    private bool TrouverPosition(Bounds b, out Vector3 pos)
    {
        for (int i = 0; i < essaisMax; i++)
        {
            var candidat = new Vector3(Random.Range(b.min.x, b.max.x), Random.Range(b.min.y, b.max.y), 0f);
            if (Valide(candidat, out pos)) return true;
        }

        pos = Vector3.zero;
        return false;
    }

    private bool TrouverPrès(Vector3 centre, out Vector3 pos)
    {
        for (int i = 0; i < essaisMax; i++)
        {
            var candidat = centre + (Vector3)(Random.insideUnitCircle * rayonGroupe);
            if (Valide(candidat, out pos)) return true;
        }

        pos = Vector3.zero;
        return false;
    }

    private bool Valide(Vector3 candidat, out Vector3 pos)
    {
        pos = Vector3.zero;

        if (!NavMesh.SamplePosition(candidat, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            return false;

        pos = hit.position;
        pos.z = 0f;

        if (!limites.OverlapPoint(pos)) return false;

        if (player.instance != null &&
            Vector2.Distance(pos, player.instance.transform.position) < distanceMinJoueur)
            return false;

        if (horsEcran && EstVisible(pos)) return false;

        return true;
    }

    private bool EstVisible(Vector3 pos)
    {
        Vector3 v = cam.WorldToViewportPoint(pos);
        return v.x > 0f && v.x < 1f && v.y > 0f && v.y < 1f;
    }
}