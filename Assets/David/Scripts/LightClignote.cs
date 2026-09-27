using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightClignote : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private Light2D light2D;

    [Header("Paramètres d'intensité")]
    [SerializeField] private float intensiteMin = 0.5f;
    [SerializeField] private float intensiteMax = 2f;

    [Header("Paramètres volumétric intensity")]
    [SerializeField] private bool activerVolumetric = false;
    [SerializeField, Range(0f, 1f)] private float volumetricIntensityMin = 0.2f;
    [SerializeField, Range(0f, 1f)] private float volumetricIntensityMax = 0.8f;

    [Header("Paramètres de temps")]
    [SerializeField] private float vitesseClignotement = 2f;
    [SerializeField] private bool utiliserCourbePersonnalisee = false;
    [SerializeField] private AnimationCurve courbeClignotement = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Décalage temporel")]
    [SerializeField] private bool utiliserDecalageAleatoire = true;
    [SerializeField] private float decalageTemporel = 0f;

    private float temps;

    void Start()
    {
        if (light2D == null)
        {
            light2D = GetComponent<Light2D>();
        }

        if (light2D == null)
        {
            Debug.LogError("Aucune Light2D trouvée sur " + gameObject.name);
            enabled = false;
            return;
        }

        // Active le volumetric si nécessaire
        if (activerVolumetric && !light2D.volumetricEnabled)
        {
            light2D.volumetricEnabled = true;
        }

        // Initialise le temps avec le décalage
        if (utiliserDecalageAleatoire)
        {
            temps = Random.Range(0f, Mathf.PI * 2f);
        }
        else
        {
            temps = decalageTemporel;
        }
    }

    void Update()
    {
        temps += Time.deltaTime * vitesseClignotement;

        float valeurNormalisee;

        if (utiliserCourbePersonnalisee)
        {
            valeurNormalisee = courbeClignotement.Evaluate(Mathf.PingPong(temps, 1f));
        }
        else
        {
            valeurNormalisee = (Mathf.Sin(temps) + 1f) / 2f;
        }

        light2D.intensity = Mathf.Lerp(intensiteMin, intensiteMax, valeurNormalisee);

        // Applique le clignotement du volumetric intensity si activé
        if (activerVolumetric)
        {
            if (!light2D.volumetricEnabled)
            {
                light2D.volumetricEnabled = true;
            }
            light2D.volumeIntensity = Mathf.Lerp(volumetricIntensityMin, volumetricIntensityMax, valeurNormalisee);
        }
    }
}
