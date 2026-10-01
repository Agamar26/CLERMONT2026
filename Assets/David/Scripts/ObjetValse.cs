using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ObjetValse : MonoBehaviour
{
    public float force = 6f;                    // vitesse d'éjection
    public float freinage = 3f;                 // plus c'est grand, plus il s'arrête vite
    public float vitesseRotation = 720f;        // degrés/seconde au départ
    public float hauteurBond = 0.4f;            // petit saut avant de retomber
    public float dureeBond = 0.35f;
    public float delaiAvantDisparition = 2f;
    public float dureeFondu = 0.5f;

    private Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;                   // ne bloque jamais le joueur
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<player>() == null) return;

        col.enabled = false;                    // un seul coup suffit
        Vector2 dir = ((Vector2)(transform.position - other.transform.position)).normalized;
        if (dir == Vector2.zero) dir = Random.insideUnitCircle.normalized;
        StartCoroutine(Valser(dir));
    }

    private IEnumerator Valser(Vector2 dir)
    {
        SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
        Vector3 posSol = transform.position;
        Vector2 vitesse = dir * force;
        float rot = vitesseRotation * (Random.value < 0.5f ? -1f : 1f);
        float total = delaiAvantDisparition + dureeFondu;

        for (float t = 0f; t < total; t += Time.deltaTime)
        {
            float dt = Time.deltaTime;

            // Glissade qui ralentit
            posSol += (Vector3)(vitesse * dt);
            vitesse = Vector2.Lerp(vitesse, Vector2.zero, freinage * dt);
            rot = Mathf.Lerp(rot, 0f, freinage * dt);

            // Bond : monte puis retombe au sol
            float h = t < dureeBond ? Mathf.Sin(t / dureeBond * Mathf.PI) * hauteurBond : 0f;
            transform.position = posSol + Vector3.up * h;
            transform.Rotate(0f, 0f, rot * dt);

            // Fondu final
            if (t > delaiAvantDisparition)
            {
                float a = 1f - (t - delaiAvantDisparition) / dureeFondu;
                foreach (var sr in srs)
                {
                    Color c = sr.color;
                    c.a = a;
                    sr.color = c;
                }
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}