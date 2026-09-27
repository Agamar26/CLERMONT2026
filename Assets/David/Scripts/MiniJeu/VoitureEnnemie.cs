using UnityEngine;

public class VoitureEnnemie : MonoBehaviour
{
    [HideInInspector] public float vitesse;

    private float limiteGauche;

    void Start()
    {
        limiteGauche = Camera.main.ViewportToWorldPoint(Vector3.zero).x - 3f;
    }

    void Update()
    {
        transform.position += Vector3.left * vitesse * Time.deltaTime;
        if (transform.position.x < limiteGauche) Destroy(gameObject);
    }
}