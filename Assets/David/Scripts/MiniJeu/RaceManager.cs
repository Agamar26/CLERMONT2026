using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance { get; private set; }

    [Header("Route")]
    public float[] voies = { -3f, -1f, 1f, 3f };   // Y des voies, du bas vers le haut
    public float vitesseDecor = 5f;
    public float dureeTrajet = 60f;
    public float routeLibreAvantArrivee = 3f;

    [Header("Musique")]
    public AudioSource musique;
    public float delaiArretMusique = 1f;   // après la fin des spawns
    public float dureeFondu = 1.5f;

    [Header("Objets à désactiver en fin de course")]
    public GameObject[] aDesactiver;

    [Header("Fin")]
    public string sceneVictoire = "Victoire";
    public string sceneDefaite = "FinAlternative";
    public float delaiAvantChargement = 1f;

    public bool Termine { get; private set; }
    public float Progression => Mathf.Clamp01(temps / dureeTrajet);
    public bool ArriveeProche => temps >= dureeTrajet - routeLibreAvantArrivee;

    private float temps;
    private bool finDeCourseLancee;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Termine) return;

        temps += Time.deltaTime;

        if (!finDeCourseLancee && temps >= dureeTrajet - routeLibreAvantArrivee + delaiArretMusique)
            FinDeCourse();

        if (temps >= dureeTrajet) Fin(sceneVictoire);
    }

    public void Perdu()
    {
        if (!Termine) Fin(sceneDefaite);
    }

    private void Fin(string scene)
    {
        Termine = true;
        StartCoroutine(Charger(scene));
    }

    private IEnumerator Charger(string scene)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(delaiAvantChargement);
        Time.timeScale = 1f;
        SceneManager.LoadScene(scene);
    }

    private void FinDeCourse()
    {
        finDeCourseLancee = true;

        foreach (var go in aDesactiver)
            if (go != null) go.SetActive(false);

        if (musique != null) StartCoroutine(Fondu());
    }

    private IEnumerator Fondu()
    {
        float volumeDepart = musique.volume;
        float t = 0f;

        while (t < dureeFondu)
        {
            t += Time.unscaledDeltaTime;
            musique.volume = Mathf.Lerp(volumeDepart, 0f, t / dureeFondu);
            yield return null;
        }

        musique.Stop();
        musique.volume = volumeDepart;
    }
}