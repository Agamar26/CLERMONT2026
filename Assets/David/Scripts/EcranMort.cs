using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EcranMort : MonoBehaviour
{
    public player joueur;                 // auto si vide
    public CanvasGroup panneau;
    public TMP_Text texte;
    public TMP_Text texteCri;
    [TextArea(2, 4)] public string message = "La mort ne m'empêchera pas\nde faire les courses pour Mamie !";
    public string cri = "SA MMÈRE !!";

    public float delaiAvantMessage = 1f;  // laisse jouer l'anim de mort
    public float dureeFondu = 0.5f;
    public float delaiCri = 0.6f;         // le cri arrive après la phrase
    public float dureeImpact = 0.25f;
    public float echelleDepart = 3f;      // le cri "tombe" de cette taille
    public float angleCri = -6f;          // inclinaison BD
    public float dureeAffichage = 3f;

    void Start()
    {
        if (joueur == null) joueur = FindFirstObjectByType<player>();
        joueur.OnMort += LancerMort;

        texte.text = message;
        texteCri.text = cri;
        texteCri.transform.localScale = Vector3.zero;

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

        yield return new WaitForSecondsRealtime(delaiCri);

        // Impact : grand → taille normale, en ease-out
        Transform cri = texteCri.transform;
        cri.localRotation = Quaternion.Euler(0f, 0f, angleCri);
        for (float t = 0f; t < dureeImpact; t += Time.unscaledDeltaTime)
        {
            float k = 1f - Mathf.Pow(1f - t / dureeImpact, 3f);
            cri.localScale = Vector3.one * Mathf.Lerp(echelleDepart, 1f, k);
            yield return null;
        }
        cri.localScale = Vector3.one;

        yield return new WaitForSecondsRealtime(dureeAffichage);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}