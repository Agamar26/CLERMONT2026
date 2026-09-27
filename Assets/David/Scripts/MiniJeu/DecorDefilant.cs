using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DecorDefilant : MonoBehaviour
{
    public float multiplicateur = 1f;   // < 1 pour les plans lointains (parallaxe)

    private float largeur;

    void Start()
    {
        largeur = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float v = RaceManager.Instance.vitesseDecor * multiplicateur;
        transform.position += Vector3.left * v * Time.deltaTime;

        if (transform.position.x <= -largeur)
            transform.position += Vector3.right * largeur * 2f;
    }
}
