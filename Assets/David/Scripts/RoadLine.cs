using UnityEngine;

public class RoadLine : MonoBehaviour
{
    private float limiteGauche;

    void Start()
    {
        limiteGauche = Camera.main.ViewportToWorldPoint(Vector3.zero).x - 3f;
    }

    void Update()
    {
        float vitesse = RaceManager.Instance != null ? RaceManager.Instance.vitesseDecor : 5f;
        transform.position += Vector3.left * vitesse * Time.deltaTime;

        if (transform.position.x < limiteGauche) Destroy(gameObject);
    }
}