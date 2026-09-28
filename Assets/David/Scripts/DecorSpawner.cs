using UnityEngine;
using System.Collections.Generic;

public class DecorSpawner : MonoBehaviour
{
    public GameObject[] decorPrefabs;
    public float espacementMin = 2f;
    public float espacementMax = 5f;
    public float decalageY = 0f;   // variation verticale aléatoire (+/-)

    private GameObject dernier;
    private float prochainEspacement;
    private readonly List<GameObject> pioche = new List<GameObject>();

    private void Start()
    {
        if (decorPrefabs.Length == 0)
        {
            Debug.LogError("Aucun prefab de décor assigné.");
            enabled = false;
            return;
        }

        // Remplit l'écran dès le départ, du spawner jusqu'au bord gauche
        float xGauche = Camera.main.ViewportToWorldPoint(Vector3.zero).x;
        float x = transform.position.x;
        while (x > xGauche - espacementMax)
        {
            dernier = Spawn(x);
            x -= Random.Range(espacementMin, espacementMax);
        }

        prochainEspacement = Random.Range(espacementMin, espacementMax);
    }

    private void Update()
    {
        if (dernier == null || transform.position.x - dernier.transform.position.x >= prochainEspacement)
        {
            float x = dernier != null ? dernier.transform.position.x + prochainEspacement : transform.position.x;
            dernier = Spawn(x);
            prochainEspacement = Random.Range(espacementMin, espacementMax);
        }
    }

    private GameObject Spawn(float x)
    {
        var pos = new Vector3(x, transform.position.y + Random.Range(-decalageY, decalageY), transform.position.z);
        return Instantiate(Piocher(), pos, Quaternion.identity);
    }

    // Mélange le tableau et le vide avant de recommencer : un maximum de variété
    private GameObject Piocher()
    {
        if (pioche.Count == 0)
        {
            pioche.AddRange(decorPrefabs);
            for (int i = pioche.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (pioche[i], pioche[j]) = (pioche[j], pioche[i]);
            }
        }

        var p = pioche[pioche.Count - 1];
        pioche.RemoveAt(pioche.Count - 1);
        return p;
    }
}