using UnityEngine;
using System.Collections.Generic;

public class BonusSpawner : MonoBehaviour
{
    public GameObject[] bonusPrefabs;
    public Transform[] spawnPoints;
    public float spawnInterval = 5f;
    public int maxBonus = 3;
    public float delaiReapparition = 10f;   // temps avant qu'un point ramassé redevienne disponible

    private GameObject[] occupants;
    private bool[] occupe;
    private float[] disponibleA;
    private float nextSpawnTime;

    void Start()
    {
        occupants = new GameObject[spawnPoints.Length];
        occupe = new bool[spawnPoints.Length];
        disponibleA = new float[spawnPoints.Length];
        nextSpawnTime = Time.time + spawnInterval;
    }

    void Update()
    {
        // Détecte les bonus ramassés et lance le délai sur leur point
        for (int i = 0; i < occupants.Length; i++)
        {
            if (occupe[i] && occupants[i] == null)
            {
                occupe[i] = false;
                disponibleA[i] = Time.time + delaiReapparition;
            }
        }

        if (Time.time < nextSpawnTime) return;

        SpawnBonus();
        nextSpawnTime = Time.time + spawnInterval;
    }

    void SpawnBonus()
    {
        if (bonusPrefabs.Length == 0 || spawnPoints.Length == 0)
        {
            Debug.LogWarning("Bonus prefabs or spawn points array is empty.");
            return;
        }

        var libres = new List<int>();
        int presents = 0;
        for (int i = 0; i < occupants.Length; i++)
        {
            if (occupe[i]) presents++;
            else if (Time.time >= disponibleA[i]) libres.Add(i);
        }

        if (presents >= maxBonus || libres.Count == 0) return;

        int point = libres[Random.Range(0, libres.Count)];
        var prefab = bonusPrefabs[Random.Range(0, bonusPrefabs.Length)];
        occupants[point] = Instantiate(prefab, spawnPoints[point].position, Quaternion.identity);
        occupe[point] = true;
    }
}