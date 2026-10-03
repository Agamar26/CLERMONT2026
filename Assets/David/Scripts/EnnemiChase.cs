using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnnemiChase : MonoBehaviour
{
    private enum State { Idle, Chase, Stun, Die }

    [Header("Références")]
    [SerializeField] private Transform visuel;      // le FBX enfant (sinon l'objet lui-même)
    [SerializeField] private Animator animator;     // sinon cherché dans les enfants
    [SerializeField] private float degatsContact;
    public float DegatsContact => degatsContact;
    public bool PeutBlesser => gelTimer <=0f && state != State.Die;
    [Header("Angle Y du modèle")]
    [SerializeField] private float angleDroite = 90f;
    [SerializeField] private float angleGauche = -90f;

    [Header("Détection (0 = illimitée)")]
    [SerializeField] private float rayonDetection = 8f;
    [SerializeField] private float rayonPerte = 11f;

    [Header("Noms des states dans l'Animator")]
    [SerializeField] private string stateIdle = "Idle";
    [SerializeField] private string stateChase = "Chase";
    [SerializeField] private string stateStun = "Stun";
    [SerializeField] private string stateDie = "Die";
    [SerializeField] private float fondu = 0.1f;

    [Header("Gel")]
    [SerializeField] private Color couleurGel = new Color(0.4f, 0.7f, 1f);
    [SerializeField, Range(0f, 1f)] private float intensiteGel = 0.7f;

    private static readonly int idBaseColor = Shader.PropertyToID("_BaseColor"); // URP
    private static readonly int idColor = Shader.PropertyToID("_Color");         // Built-in

    private struct MatCouleur
    {
        public Material mat;
        public int prop;
        public Color origine;
    }

    private NavMeshAgent agent;
    private State state;
    private float stunTimer;
    private float gelTimer;
    private float vitesseAnimAvantGel = 1f;
    private readonly List<MatCouleur> materiaux = new List<MatCouleur>();

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        transform.rotation = Quaternion.identity;
        agent.enabled = true;

        if (visuel == null) visuel = transform;
        if (animator == null) animator = GetComponentInChildren<Animator>();

        // Matériaux du modèle 3D uniquement (la barre de vie, en sprites, n'est pas teintée)
        foreach (Renderer r in GetComponentsInChildren<Renderer>(true))
        {
            if (!(r is MeshRenderer || r is SkinnedMeshRenderer)) continue;

            foreach (Material m in r.materials)
            {
                int prop = m.HasProperty(idBaseColor) ? idBaseColor : m.HasProperty(idColor) ? idColor : -1;
                if (prop == -1) continue;
                materiaux.Add(new MatCouleur { mat = m, prop = prop, origine = m.GetColor(prop) });
            }
        }
    }

    void Start()
    {
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
                agent.SetDestination(playerPos);
                Orienter(playerPos.x - transform.position.x);
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
        foreach (MatCouleur mc in materiaux)
        {
            Color c = gele ? Color.Lerp(mc.origine, couleurGel, intensiteGel) : mc.origine;
            mc.mat.SetColor(mc.prop, c);
        }
    }

    private bool JoueurDetecte()
    {
        if (player.instance == null) return false;
        if (rayonDetection <= 0f) return true;

        float r = state == State.Chase ? rayonPerte : rayonDetection;
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
        if (!mobile) agent.ResetPath();

        if (animator == null) return;

        string nom = nouveau switch
        {
            State.Idle => stateIdle,
            State.Chase => stateChase,
            State.Stun => stateStun,
            _ => stateDie
        };
        animator.CrossFadeInFixedTime(nom, fondu);
    }
}