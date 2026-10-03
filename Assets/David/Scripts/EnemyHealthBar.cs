using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.2f, 0f);
    [SerializeField] private float largeur = 1f;
    [SerializeField] private float hauteur = 0.12f;
    [SerializeField] private bool cacherSiPlein = true;

    [Header("Couleurs")]
    [SerializeField] private Color couleurFond = new Color(0f, 0f, 0f, 0.6f);
    [SerializeField] private Color couleurPleine = Color.green;
    [SerializeField] private Color couleurVide = Color.red;

    private static Sprite carre;

    private EnemyStats stats;
    private GameObject barre;
    private SpriteRenderer remplissage;

    void Awake()
    {
        stats = GetComponent<EnemyStats>();

        // Sprite carré blanc de 1 unité, pivot à gauche (pour remplir de gauche à droite)
        if (carre == null)
        {
            Texture2D tex = Texture2D.whiteTexture;
            carre = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 0.5f), tex.width);
        }

        barre = new GameObject("BarreVie");
        barre.transform.SetParent(transform, false);
        barre.transform.localPosition = offset;

        CreerCalque("Fond", couleurFond, 100).transform.localScale = new Vector3(largeur, hauteur, 1f);
        remplissage = CreerCalque("Remplissage", couleurPleine, 101);
    }

    void Update()
    {
        float ratio = stats.MaxHealth > 0f ? stats.Health / stats.MaxHealth : 0f;

        barre.SetActive(!(cacherSiPlein && ratio >= 1f));

        remplissage.transform.localScale = new Vector3(largeur * ratio, hauteur, 1f);
        remplissage.color = Color.Lerp(couleurVide, couleurPleine, ratio);
    }

    private SpriteRenderer CreerCalque(string nom, Color couleur, int ordre)
    {
        GameObject go = new GameObject(nom);
        go.transform.SetParent(barre.transform, false);
        go.transform.localPosition = new Vector3(-largeur / 2f, 0f, 0f);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = carre;
        sr.color = couleur;
        sr.sortingOrder = ordre;
        return sr;
    }
}