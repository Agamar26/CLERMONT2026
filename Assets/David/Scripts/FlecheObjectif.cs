using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(SpriteRenderer))]
public class FlecheObjectif : MonoBehaviour
{
    public Transform joueur;
    [FormerlySerializedAs("sortie")]
    public Transform cible;
    public float distance = 1.5f;              // écart entre le joueur et la flèche
    public float vitesseClignotement = 4f;
    public float distanceMasquage = 2f;        // la flèche disparaît quand le joueur est proche de la cible

    private SpriteRenderer sr;
    private bool active;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
    }

    public void Activer(Transform nouvelleCible = null)
    {
        if (nouvelleCible != null) cible = nouvelleCible;
        active = true;
    }

    public void Desactiver()
    {
        active = false;
        sr.enabled = false;
    }

    void LateUpdate()
    {
        // Cible détruite ou désactivée : on l'oublie et on cache la flèche
        if (cible != null && !cible.gameObject.activeInHierarchy) cible = null;

        if (!active || joueur == null || cible == null)
        {
            sr.enabled = false;
            return;
        }

        Vector2 delta = cible.position - joueur.position;
        sr.enabled = delta.magnitude > distanceMasquage;
        if (!sr.enabled) return;

        Vector2 dir = delta.normalized;
        transform.position = (Vector2)joueur.position + dir * distance;
        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

        Color c = sr.color;
        c.a = Mathf.Lerp(0.2f, 1f, Mathf.PingPong(Time.time * vitesseClignotement, 1f));
        sr.color = c;
    }
}