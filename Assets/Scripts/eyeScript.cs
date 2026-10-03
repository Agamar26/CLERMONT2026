using System.Collections.Generic;
using UnityEngine;

public class eyeScript : MonoBehaviour
{
    public bool launched;
    public List<Vector2> origin = new List<Vector2>();
    public List<Vector2> direction = new List<Vector2>();
    public float divise;
    public float timerCanGetBall;
    public LayerMask nulllayermask;

    [Header("Dégâts")]
    public float degats = 5f;

    [Header("Récupération")]
    public float rayonRecup = 1.5f;
    public float vitesseRetour = 20f;

    [Header("Retour automatique (0 = désactivé)")]
    public int rebondsMax = 3;
    public float dureeMax = 3f;

    private bool attrape;
    private int rebonds;
    private float tempsEnVol;

    void Start()
    {
        divise = 1;
    }

    void Update()
    {
        for (int i = 0; i < origin.Count; i++)
        {
            Debug.DrawRay(origin[i], direction[i], Color.red);
        }
        if (!launched) { return; }

        if (!attrape)
        {
            tempsEnVol += Time.deltaTime;
            if (dureeMax > 0f && tempsEnVol >= dureeMax) Attraper();
        }

        if (timerCanGetBall < 0.5f)
        {
            timerCanGetBall += Time.deltaTime;
            return;
        }

        if (!attrape && Vector2.Distance(transform.position, player.instance.transform.position) < rayonRecup)
        {
            Attraper();
        }

        if (!attrape) { return; }

        Transform cible = player.instance.eyePos;
        transform.localEulerAngles = Vector3.MoveTowards(transform.localEulerAngles, cible.localEulerAngles, vitesseRetour * Time.deltaTime);
        transform.position = Vector2.MoveTowards(transform.position, cible.position, vitesseRetour * Time.deltaTime);

        if (Vector2.Distance(transform.position, cible.position) < 0.05f)
        {
            cible.GetComponent<SpriteRenderer>().enabled = true;
            player.instance.donthaveEye = false;

            if (player.instance.state == player.playerstate.confused)
            {
                player.instance.state = player.playerstate.idle;
            }

            Destroy(gameObject);
        }
    }

    // Coupe la physique : l'œil traverse les murs pour revenir au joueur
    private void Attraper()
    {
        attrape = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!launched || attrape) { return; }

        bool estEnnemi = collision.gameObject.CompareTag("Enemy");
        if (!collision.gameObject.CompareTag("Ground") && !estEnnemi) { return; }

        if (estEnnemi)
        {
            EnemyStats stats = collision.gameObject.GetComponentInParent<EnemyStats>();
            if (stats != null) stats.TakeDamage(degats / divise);
            collision.gameObject.GetComponentInParent<EnnemiChase>()?.Stun(0.5f);
        }

        rebonds++;
        if (rebondsMax > 0 && rebonds >= rebondsMax)
        {
            Attraper();
            return;
        }

        Vector2 contact = collision.contacts[0].point;
        Vector2 eze = Vector2.Reflect((contact - origin[origin.Count - 1]).normalized, collision.contacts[0].normal);
        origin.Add(contact);
        direction.Add(eze);
        GetComponent<Rigidbody2D>().linearVelocity = eze.normalized * (player.instance.forceLaunch / divise);
        divise += 0.5f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, rayonRecup);
    }
}