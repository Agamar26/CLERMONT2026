using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DecorDefilant : MonoBehaviour
{
    public float multiplicateur = 1f;   // < 1 pour les plans lointains (parallaxe)

    [Tooltip("Nombre de copies du sprite côte à côte pour ce plan")]
    public int nombreTuiles = 2;

    [Tooltip("Marge hors écran avant de replacer la tuile")]
    public float marge = 0.5f;

    private SpriteRenderer sr;
    private Camera cam;
    private float largeur;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        cam = Camera.main;
        largeur = sr.bounds.size.x;
    }

    void LateUpdate()
    {
        float v = RaceManager.Instance.vitesseDecor * multiplicateur;
        transform.position += Vector3.left * v * Time.deltaTime;

        float bordGauche = cam.transform.position.x - cam.orthographicSize * cam.aspect;

        // bord droit de la tuile entièrement sorti à gauche de l'écran
        if (sr.bounds.max.x < bordGauche - marge)
            transform.position += Vector3.right * largeur * nombreTuiles;
    }
}