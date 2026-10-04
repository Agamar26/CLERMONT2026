using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EcranMort : MonoBehaviour
{
    public player joueur;                 // auto si vide
    public CanvasGroup panneau;
    public TMP_Text texte;
    [TextArea(3, 5)]
    public string message =
        "La mort ne m'empêchera pas de faire les courses pour Mamie !\n<size=150%><b>SA MMÈRE !!</b></size>";
    public float delaiAvantMessage = 1f;  // laisse jouer l'anim de mort
    public float dureeFondu = 0.5f;
    public float dureeAffichage = 3f;

    void Start()
    {
        if (joueur == null) joueur = FindFirstObjectByType<player>();
        joueur.OnMort += LancerMort;

        texte.text = message;
        panneau.alpha = 0f;
        panneau.blocksRaycasts = false;
    }

    void OnDestroy()
    {
        if (joueur != null) joueur.OnMort -= LancerMort;
    }

    private void LancerMort()
    {
        StartCoroutine(SequenceMort());
    }

    // Temps réel : insensible à un éventuel timeScale à 0
    private IEnumerator SequenceMort()
    {
        yield return new WaitForSecondsRealtime(delaiAvantMessage);

        for (float t = 0f; t < dureeFondu; t += Time.unscaledDeltaTime)
        {
            panneau.alpha = t / dureeFondu;
            yield return null;
        }
        panneau.alpha = 1f;

        yield return new WaitForSecondsRealtime(dureeAffichage);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}