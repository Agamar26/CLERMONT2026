using TMPro;
using UnityEngine;

public class BullePNJ : MonoBehaviour
{
    public GameObject bulle;              // enfant du PNJ
    public TMP_Text texte;                // accepte TMP UI (Canvas) ou TMP 3D
    [TextArea] public string message = "Merci pour les courses !";
    public float amplitude = 0.05f;       // flottement vertical
    public float vitesse = 2f;

    private Vector3 posBase;
    private bool declenche;

    void Start()
    {
        posBase = bulle.transform.localPosition;
        if (texte != null) texte.text = message;
        bulle.SetActive(false);
    }

    void Update()
    {
        if (declenche)
            bulle.transform.localPosition = posBase + Vector3.up * Mathf.Sin(Time.time * vitesse) * amplitude;
    }

    // Stay plutôt qu'Enter : marche aussi si les courses se terminent alors que le joueur est déjà dans la zone
    void OnTriggerStay2D(Collider2D other)
    {
        if (declenche || !gameManager.instance.CourseOk) return;
        if (other.GetComponentInParent<player>() == null) return;

        declenche = true;
        bulle.SetActive(true);
    }
}