using UnityEngine;

public class RoadLinesSpawner : MonoBehaviour
{
    public GameObject roadLinePrefab;
    public float espacement = 3f;   // distance entre deux lignes

    private GameObject derniere;

    private void Start()
    {
        if (roadLinePrefab == null)
        {
            Debug.LogError("Road line prefab is not assigned.");
            enabled = false;
            return;
        }

        // Remplit l'écran dès le départ, du spawner jusqu'au bord gauche
        float xGauche = Camera.main.ViewportToWorldPoint(Vector3.zero).x;
        for (float x = transform.position.x; x > xGauche - espacement; x -= espacement)
            derniere = Spawn(x);
    }

    private void Update()
    {
        // Nouvelle ligne dès que la précédente s'est assez éloignée du spawner
        if (derniere == null || transform.position.x - derniere.transform.position.x >= espacement)
        {
            float x = derniere != null ? derniere.transform.position.x + espacement : transform.position.x;
            derniere = Spawn(x);
        }
    }

    private GameObject Spawn(float x)
    {
        var pos = new Vector3(x, transform.position.y, transform.position.z);
        return Instantiate(roadLinePrefab, pos, Quaternion.identity);
    }
}