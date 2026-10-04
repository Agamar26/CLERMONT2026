using System.Collections;
using UnityEngine;

public class DoorIntereieure : MonoBehaviour
{
    public Animator animator;             // auto si vide
    public string stateOpen = "Open";
    public float dureeFermeture = 0.4f;

    private Transform[] pieces;
    private Vector3[] posFermee, scaleFermee;
    private Quaternion[] rotFermee;
    private SpriteRenderer[] rendus;
    private Sprite[] spritesFermes;
    private Coroutine fermeture;
    private bool ouverte;

    void Awake()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();

        // Pose fermée = pose de départ dans la scène, pour l'Animator et tous ses enfants
        pieces = animator.GetComponentsInChildren<Transform>(true);
        posFermee = new Vector3[pieces.Length];
        rotFermee = new Quaternion[pieces.Length];
        scaleFermee = new Vector3[pieces.Length];
        for (int i = 0; i < pieces.Length; i++)
        {
            posFermee[i] = pieces[i].localPosition;
            rotFermee[i] = pieces[i].localRotation;
            scaleFermee[i] = pieces[i].localScale;
        }

        rendus = animator.GetComponentsInChildren<SpriteRenderer>(true);
        spritesFermes = new Sprite[rendus.Length];
        for (int i = 0; i < rendus.Length; i++) spritesFermes[i] = rendus[i].sprite;

        // Fermée au départ : l'Animator ne tourne qu'à l'ouverture
        animator.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (ouverte || other.GetComponentInParent<player>() == null) return;
        ouverte = true;

        if (fermeture != null) StopCoroutine(fermeture);
        animator.enabled = true;
        animator.Play(stateOpen, 0, 0f);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!ouverte || other.GetComponentInParent<player>() == null) return;
        ouverte = false;

        fermeture = StartCoroutine(Fermer());
    }

    private IEnumerator Fermer()
    {
        animator.enabled = false; // sinon l'Animator réécrase les transforms à chaque frame

        int n = pieces.Length;
        var p0 = new Vector3[n];
        var r0 = new Quaternion[n];
        var s0 = new Vector3[n];
        for (int i = 0; i < n; i++)
        {
            p0[i] = pieces[i].localPosition;
            r0[i] = pieces[i].localRotation;
            s0[i] = pieces[i].localScale;
        }

        for (float t = 0f; t < dureeFermeture; t += Time.deltaTime)
        {
            float k = Mathf.SmoothStep(0f, 1f, t / dureeFermeture);
            for (int i = 0; i < n; i++)
            {
                pieces[i].localPosition = Vector3.Lerp(p0[i], posFermee[i], k);
                pieces[i].localRotation = Quaternion.Slerp(r0[i], rotFermee[i], k);
                pieces[i].localScale = Vector3.Lerp(s0[i], scaleFermee[i], k);
            }
            yield return null;
        }

        for (int i = 0; i < n; i++)
        {
            pieces[i].localPosition = posFermee[i];
            pieces[i].localRotation = rotFermee[i];
            pieces[i].localScale = scaleFermee[i];
        }
        for (int i = 0; i < rendus.Length; i++) rendus[i].sprite = spritesFermes[i];

        fermeture = null;
    }
}