using UnityEngine;
using System.Collections.Generic;

public class SpawnerVoitures : MonoBehaviour
{
    public GameObject[] voitures;
    public float intervalleMin = 0.6f;
    public float intervalleMax = 1.5f;
    public float vitesseMin = 3f;
    public float vitesseMax = 8f;
    public float delaiMemeVoie = 1.2f;

    private float xSpawn;
    private float prochain;
    private float[] libreA;
    private VoitureEnnemie[] derniere;

    void Start()
    {
        xSpawn = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0f)).x + 2f;

        int n = RaceManager.Instance.voies.Length;
        libreA = new float[n];
        derniere = new VoitureEnnemie[n];
    }

    void Update()
    {
        var rm = RaceManager.Instance;
        if (rm.Termine || rm.ArriveeProche || Time.time < prochain) return;
        prochain = Time.time + Random.Range(intervalleMin, intervalleMax);

        var libres = new List<int>();
        for (int i = 0; i < libreA.Length; i++)
            if (Time.time >= libreA[i]) libres.Add(i);

        // Toujours garder au moins une voie libre
        if (libres.Count <= 1) return;

        int v = libres[Random.Range(0, libres.Count)];
        libreA[v] = Time.time + delaiMemeVoie;

        var go = Instantiate(voitures[Random.Range(0, voitures.Length)],
                             new Vector3(xSpawn, rm.voies[v], 0f), Quaternion.identity);

        var car = go.GetComponent<VoitureEnnemie>();
        car.vitesse = Random.Range(vitesseMin, vitesseMax);
        if (derniere[v] != null) car.vitesse = Mathf.Min(car.vitesse, derniere[v].vitesse);
        derniere[v] = car;
    }
}