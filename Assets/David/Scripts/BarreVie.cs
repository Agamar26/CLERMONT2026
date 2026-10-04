using UnityEngine;
using UnityEngine.UI;

public class BarreVie : MonoBehaviour
{
    [Header("Images (Image Type = Filled, Horizontal, Origin Left)")]
    public Image remplissage;
    public Image trainee;                 // derrière le remplissage
    public Gradient couleur;              // 0 = vide → 1 = pleine

    [Header("Animation")]
    public float vitesseRemplissage = 10f;
    public float delaiTrainee = 0.4f;     // la traînée attend avant de descendre
    public float vitesseTrainee = 1.2f;   // en part de barre par seconde

    [Header("Secousse au coup")]
    public RectTransform cadre;           // auto = cet objet
    public float forceSecousse = 6f;
    public float dureeSecousse = 0.2f;

    [Header("Vie critique")]
    [Range(0f, 1f)] public float seuilCritique = 0.25f;
    public float vitessePulse = 8f;

    private float affiche = 1f, valeurTrainee = 1f, derniere = 1f;
    private float timerTrainee, timerSecousse;
    private Vector2 posBase;

    // Valeurs par défaut quand on ajoute le composant
    void Reset()
    {
        couleur = new Gradient();
        couleur.SetKeys(
            new[]
            {
                new GradientColorKey(new Color(0.85f, 0.15f, 0.15f), 0f),
                new GradientColorKey(new Color(1f, 0.8f, 0.1f), 0.5f),
                new GradientColorKey(new Color(0.3f, 0.85f, 0.3f), 1f)
            },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
    }

    void Start()
    {
        if (cadre == null) cadre = (RectTransform)transform;
        posBase = cadre.anchoredPosition;

        affiche = valeurTrainee = derniere = Ratio();
        Appliquer();
    }

    void Update()
    {
        float r = Ratio();

        if (r < derniere)
        {
            timerTrainee = delaiTrainee;   // coup reçu
            timerSecousse = dureeSecousse;
        }
        derniere = r;

        // Remplissage : suit la vie en douceur (indépendant du framerate)
        affiche = Mathf.Lerp(affiche, r, 1f - Mathf.Exp(-vitesseRemplissage * Time.deltaTime));

        // Traînée : attend, puis rattrape. Jamais en dessous du remplissage (soins)
        if (timerTrainee > 0f) timerTrainee -= Time.deltaTime;
        else valeurTrainee = Mathf.MoveTowards(valeurTrainee, affiche, vitesseTrainee * Time.deltaTime);
        valeurTrainee = Mathf.Max(valeurTrainee, affiche);

        // Secousse qui s'amortit
        if (timerSecousse > 0f)
        {
            timerSecousse -= Time.deltaTime;
            float k = Mathf.Clamp01(timerSecousse / dureeSecousse);
            cadre.anchoredPosition = posBase + Random.insideUnitCircle * forceSecousse * k;
        }
        else
        {
            cadre.anchoredPosition = posBase;
        }

        Appliquer();
    }

    private float Ratio()
    {
        player p = player.instance;
        return p == null ? 1f : Mathf.Clamp01(p.health / p.maxHealth);
    }

    private void Appliquer()
    {
        remplissage.fillAmount = affiche;
        trainee.fillAmount = valeurTrainee;

        Color c = couleur.Evaluate(affiche);
        if (affiche > 0f && affiche <= seuilCritique)
        {
            float pulse = (Mathf.Sin(Time.time * vitessePulse) + 1f) * 0.5f;
            c = Color.Lerp(c, Color.white, pulse * 0.5f);
        }
        remplissage.color = c;
    }
}