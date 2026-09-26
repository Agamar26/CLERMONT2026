using UnityEngine;
using System.Collections.Generic;

public class BonusSpawner : MonoBehaviour
{
    public GameObject[] bonusPrefabs;
    public Transform[] spawnPoints;
    public float spawnInterval = 5f;
    public int maxBonus = 3;

    private GameObject[] occupants;
    private float nextSpawnTime;

    void Start()
    {
        occupants = new GameObject[spawnPoints.Length];
        nextSpawnTime = Time.time + spawnInterval;
    }

    void Update()
    {
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

        // Points libres (un bonus détruit devient null)
        var libres = new List<int>();
        int presents = 0;
        for (int i = 0; i < occupants.Length; i++)
        {
            if (occupants[i] == null) libres.Add(i);
            else presents++;
        }

        if (presents >= maxBonus || libres.Count == 0) return;

        int point = libres[Random.Range(0, libres.Count)];
        var prefab = bonusPrefabs[Random.Range(0, bonusPrefabs.Length)];
        occupants[point] = Instantiate(prefab, spawnPoints[point].position, Quaternion.identity);
    }
}