using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Fondu : MonoBehaviour
{
    public static float duree = 0.5f;

    private static Fondu instance;
    private CanvasGroup groupe;
    private bool enCours;

    // Créé automatiquement au lancement du jeu, avant la première scène
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Creer()
    {
        var go = new GameObject("Fondu");
        DontDestroyOnLoad(go);
        instance = go.AddComponent<Fondu>();

        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;   // toujours au-dessus du reste

        instance.groupe = go.AddComponent<CanvasGroup>();
        instance.groupe.blocksRaycasts = false;

        var noir = new GameObject("Noir").AddComponent<Image>();
        noir.transform.SetParent(go.transform, false);
        noir.color = Color.black;
        var rt = noir.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;

        // Le jeu démarre au noir puis apparaît en fondu
        instance.groupe.alpha = 1f;
        instance.StartCoroutine(instance.Vers(0f));
    }

    public static void Charger(string scene)
    {
        if (instance.enCours) return;
        instance.StartCoroutine(instance.Transition(scene));
    }

    private IEnumerator Transition(string scene)
    {
        enCours = true;
        groupe.blocksRaycasts = true;   // bloque les clics pendant la transition

        yield return Vers(1f);

        var op = SceneManager.LoadSceneAsync(scene);
        while (!op.isDone) yield return null;

        yield return Vers(0f);

        groupe.blocksRaycasts = false;
        enCours = false;
    }

    // Temps réel : fonctionne même quand le jeu est en pause (timeScale = 0)
    private IEnumerator Vers(float cible)
    {
        float depart = groupe.alpha;
        float t = 0f;

        while (t < duree)
        {
            t += Time.unscaledDeltaTime;
            groupe.alpha = Mathf.Lerp(depart, cible, t / duree);
            yield return null;
        }

        groupe.alpha = cible;
    }
}