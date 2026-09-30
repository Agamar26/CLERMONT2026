using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnnemiChase : MonoBehaviour
{
    private enum State { Idle, Chase, Stun, Die }

    [Header("Références")]
    [SerializeField] private Transform visuel;      // le FBX enfant (sinon l'objet lui-même)
    [SerializeField] private Animator animator;     // sinon cherché dans les enfants

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

    private NavMeshAgent agent;
    private State state;
    private float stunTimer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        transform.rotation = Quaternion.identity;
        agent.enabled = true;

        if (visuel == null) visuel = transform;
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        SetState(State.Idle);
    }

    void Update()
    {
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

    // --- Interne ---

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