using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnnemiChase : MonoBehaviour
{
    private enum State { Idle, Chase, Photo, Stun, Die }

    [Header("Références")]
    [SerializeField] private Transform visuel;      // le modèle enfant (sinon l'objet lui-même)
    [SerializeField] private Animator animator;     // sinon cherché dans les enfants

    [Header("Angle Y du modèle")]
    [SerializeField] private float angleDroite = 90f;
    [SerializeField] private float angleGauche = -90f;

    [Header("Détection (0 = illimitée)")]
    [SerializeField] private float rayonDetection = 8f;
    [SerializeField] private float rayonPerte = 11f;

    [Header("Photo (contact = rayonContact du joueur)")]
    [SerializeField] private float margePhoto = 0.3f; // évite d'alterner Photo/Chase à la limite

    [Header("Noms des states dans l'Animator")]
    [SerializeField] private string stateIdle = "Idle";
    [SerializeField] private string stateChase = "Chase";
    [SerializeField] private string statePhoto = "Photo";
    [SerializeField] private string stateStun = "Idle"; // pas d'état Stun dans l'Animator
    [SerializeField] private string stateDie = "Die";
    [SerializeField] private float fondu = 0.1f;

    [Header("Gel")]
    [SerializeField] private Color couleurGel = new Color(0.4f, 0.7f, 1f);
    [SerializeField, Range(0f, 1f)] private float intensiteGel = 0.7f;

    [Header("Dégâts au contact")]
    [SerializeField] private float degatsContact = 10f;

    public float DegatsContact => degatsContact;
    public bool PeutBlesser => gelTimer <= 0f && state != State.Die;

    private NavMeshAgent agent;
    private State state;
    private float stunTimer;
    private float gelTimer;
    private float vitesseAnimAvantGel = 1f;
    private SpriteRenderer[] sprites;
    private Color[] couleursOrigine;
    private Collider2D monCollider;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        transform.rotation = Quaternion.identity;
        agent.enabled = true;

        if (visuel == null) visuel = transform;
        if (animator == null) animator = GetComponentInChildren<Animator>();
        monCollider = GetComponentInChildren<Collider2D>();
    }

    void Start()
    {
        // Récupérés en Start : la barre de vie (créée en Awake) existe déjà et peut être exclue
        var liste = new List<SpriteRenderer>();
        foreach (SpriteRenderer sr in visuel.GetComponentsInChildren<SpriteRenderer>(true))
        {
            if (sr.transform.parent != null && sr.transform.parent.name == "BarreVie") continue;
            liste.Add(sr);
        }
        sprites = liste.ToArray();
        couleursOrigine = new Color[sprites.Length];
        for (int i = 0; i < sprites.Length; i++) couleursOrigine[i] = sprites[i].color;

        SetState(State.Idle);
    }

    void Update()
    {
        // Gelé : plus rien ne bouge tant que le timer tourne
        if (gelTimer > 0f)
        {
            gelTimer -= Time.deltaTime;
            if (gelTimer <= 0f) FinGel();
            return;
        }

        switch (state)
        {
            case State.Idle:
                if (JoueurDetecte()) SetState(State.Chase);
                break;

            case State.Chase:
                if (!JoueurDetecte())
                {
                    SetState(State.Idle);
                    break;
                }
                Vector3 playerPos = player.instance.transform.position;
                Orienter(playerPos.x - transform.position.x);

                if (AuContact(0f))
                {
                    SetState(State.Photo);
                    break;
                }
                agent.SetDestination(playerPos);
                break;

            case State.Photo:
                if (!JoueurDetecte())
                {
                    SetState(State.Idle);
                    break;
                }
                Orienter(player.instance.transform.position.x - transform.position.x);
                if (!AuContact(margePhoto)) SetState(State.Chase);
                break;

            case State.Stun:
                stunTimer -= Time.deltaTime;
                if (stunTimer <= 0f) SetState(State.Chase);
                break;

            case State.Die:
                break;
        }
    }

    // --- API publique ---

    public void Stun(float duree)
    {
        if (state == State.Die) return;
        stunTimer = duree;
        SetState(State.Stun);
    }

    public void Die()
    {
        if (state == State.Die) return;
        SetState(State.Die);
    }

    // Arrêt complet + teinte bleue + animation figée. Un nouveau gel prolonge le gel en cours.
    public void Geler(float duree)
    {
        if (state == State.Die) return;

        if (gelTimer <= 0f) DebutGel();
        gelTimer = Mathf.Max(gelTimer, duree);
    }

    // --- Interne ---

    // Même test que les dégâts côté joueur : le cercle rayonContact touche-t-il mon collider ?
    private bool AuContact(float marge)
    {
        Vector2 p = player.instance.transform.position;
        float r = player.instance.rayonContact + marge;

        Vector2 pointProche = monCollider != null ? monCollider.ClosestPoint(p) : (Vector2)transform.position;
        return Vector2.Distance(pointProche, p) <= r;
    }

    private void DebutGel()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        if (animator != null)
        {
            vitesseAnimAvantGel = animator.speed;
            animator.speed = 0f;
        }

        Teinter(true);
    }

    private void FinGel()
    {
        gelTimer = 0f;
        if (animator != null) animator.speed = vitesseAnimAvantGel;
        Teinter(false);
        SetState(JoueurDetecte() ? State.Chase : State.Idle);
    }

    private void Teinter(bool gele)
    {
        if (sprites == null) return;

        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].color = gele ? Color.Lerp(couleursOrigine[i], couleurGel, intensiteGel) : couleursOrigine[i];
        }
    }

    private bool JoueurDetecte()
    {
        if (player.instance == null) return false;
        if (rayonDetection <= 0f) return true;

        bool engage = state == State.Chase || state == State.Photo;
        float r = engage ? rayonPerte : rayonDetection;
        Vector3 delta = player.instance.transform.position - transform.position;
        return delta.sqrMagnitude <= r * r;
    }

    private void Orienter(float dx)
    {
        if (Mathf.Abs(dx) < 0.01f) return;
        visuel.localRotation = Quaternion.Euler(0f, dx > 0f ? angleDroite : angleGauche, 0f);
    }

    private void SetState(State nouveau)
    {
        state = nouveau;

        bool mobile = nouveau == State.Chase;
        agent.isStopped = !mobile;
        if (!mobile)
        {
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }

        if (animator == null) return;

        string nom = nouveau switch
        {
            State.Idle => stateIdle,
            State.Chase => stateChase,
            State.Photo => statePhoto,
            State.Stun => stateStun,
            _ => stateDie
        };
        animator.CrossFadeInFixedTime(nom, fondu);
    }
}
