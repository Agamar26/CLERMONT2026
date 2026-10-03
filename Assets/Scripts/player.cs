using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{
    public static player instance;
    public Rigidbody2D rb;
    public Vector2 move;
    public GameObject caddie;
    public GameObject chapeau;
    public GameObject imper;
    public GameObject lunettes;
    public float speed;
    public float basespeed;
    public float speedBonus;
    public float bonusTime = 3f;
    public float bonusTimer;
    public float stamina = 100f;
    public float maxStamina = 100f;
    public float health = 100f;
    public float maxHealth = 100f;
    public float radiusDetectGround;
    public LayerMask layerGround;
    public bool isGrounded;
    public float rotY;
    public Camera camera;
    public float cameraSpeed;
    public PlayerInput inputPlayer;
    public Animator animatorClim;
    public enum playerstate { idle, stuck, frost, confused, prelaunch, launch, invincible };
    public playerstate state;
    public bool cantMove;
    public GameObject particlesFrost;
    public Transform brasPoint;
    public float rotbras;
    public Vector2 launchdirection;
    public float forceLaunch;
    public bool donthaveEye;
    public Transform eyePos;
    public float timerbarFrost, timeBarFrost;

    [Header("Gel de zone")]
    public float rayonGel = 3f;
    [Range(0f, 1f)] public float facteurGel = 0.3f; // 0.3 = 30 % de la vitesse
    public float dureeGel = 2f;

    [Header("Caddie")]
    public float rayonCaddie = 1f;

    [Header("Flèche vers l'œil lancé")]
    public FlecheObjectif flecheOeil;
    public string tagOeil = "Oeil";

    [System.Serializable]
    public class SonEtat
    {
        public playerstate etat;
        public AudioClip clip;
        public bool boucle = true;
        [Range(0f, 1f)] public float volume = 1f;
    }

    [Header("Sons par état (un état sans entrée = silence)")]
    public AudioSource audioSource;
    public List<SonEtat> sonsEtats = new List<SonEtat>();

    private Coroutine deguisement;
    private playerstate etatPrecedent;
    private bool etatInitialise;
    private Vector2 derniereDirection = Vector2.right;
    private readonly HashSet<EnemyStats> ennemisGeles = new HashSet<EnemyStats>();

    void Start()
    {
        Time.timeScale = 1f;
        if (instance == null) instance = this;

        if (audioSource != null) audioSource.playOnAwake = false;
        if (flecheOeil != null && flecheOeil.joueur == null) flecheOeil.joueur = transform;
    }

    void Update()
    {
        if (instance == null) instance = this;

        switch (state)
        {
            case playerstate.idle:
                timerbarFrost = Mathf.MoveTowards(timerbarFrost, timeBarFrost, Time.deltaTime);
                speed = basespeed;
                if (Mathf.Round(move.x) != 0 || Mathf.Round(move.y) != 0)
                {
                    animatorClim.SetBool("Run", true);
                    derniereDirection = new Vector2(Mathf.Round(move.x), Mathf.Round(move.y)).normalized;
                    if (Mathf.Round(move.x) < 0) rotY = 180;
                    else if (Mathf.Round(move.x) > 0) rotY = 0;
                }
                else
                {
                    animatorClim.SetBool("Run", false);
                }

                transform.rotation = Quaternion.Euler(0, rotY, 0);
                OrienterBras(derniereDirection);
                isGrounded = Physics2D.OverlapCircle(transform.position - new Vector3(0, GetComponent<CapsuleCollider2D>().size.y / 2, 0), radiusDetectGround, layerGround);
                break;

            case playerstate.confused:
                timerbarFrost = Mathf.MoveTowards(timerbarFrost, timeBarFrost, Time.deltaTime);
                if (Mathf.Round(move.x) != 0 || Mathf.Round(move.y) != 0)
                {
                    animatorClim.SetBool("Run", true);
                    if (Mathf.Round(move.x) < 0) rotY = 0;
                    else if (Mathf.Round(move.x) > 0) rotY = 180;
                }
                else
                {
                    animatorClim.SetBool("Run", false);
                }

                transform.rotation = Quaternion.Euler(0, rotY, 0);
                isGrounded = Physics2D.OverlapCircle(transform.position - new Vector3(0, GetComponent<CapsuleCollider2D>().size.y / 2, 0), radiusDetectGround, layerGround);
                break;

            case playerstate.stuck:
            case playerstate.frost:
                animatorClim.SetBool("Run", false);
                break;

            case playerstate.invincible:
                speed = speedBonus;
                caddie.SetActive(true);
                animatorClim.SetBool("Run", false);
                EcraserEnnemis();

                if (bonusTimer != bonusTime)
                {
                    bonusTimer = Mathf.MoveTowards(bonusTimer, bonusTime, Time.deltaTime);
                }
                else
                {
                    caddie.SetActive(false);
                    state = donthaveEye ? playerstate.confused : playerstate.idle;
                }
                if (Mathf.Round(move.x) < 0) rotY = 180;
                else if (Mathf.Round(move.x) > 0) rotY = 0;

                transform.rotation = Quaternion.Euler(0, rotY, 0);
                break;
        }

        UpdateSonEtat();
        UpdateFlecheOeil();
    }

    // --- Lancer de l'œil : tout droit dans la dernière direction de déplacement ---

    private void OrienterBras(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        brasPoint.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void LancerOeil(InputAction.CallbackContext context)
    {
        if (donthaveEye) return;
        if (state != playerstate.idle && state != playerstate.frost) return;

        launchdirection = derniereDirection;
        OrienterBras(launchdirection);
        animatorClim.SetTrigger("Launch");
    }

    // --- Caddie : détruit tout ennemi touché ---

    private void EcraserEnnemis()
    {
        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, rayonCaddie))
        {
            EnemyStats ennemi = col.GetComponentInParent<EnemyStats>();
            if (ennemi != null && !ennemi.IsDead) ennemi.TakeDamage(ennemi.Health);
        }
    }

    // --- Gel de zone ---

    private void Geler()
    {
        ennemisGeles.Clear();

        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, rayonGel))
        {
            EnemyStats ennemi = col.GetComponentInParent<EnemyStats>();
            if (ennemi != null && ennemisGeles.Add(ennemi))
            {
                ennemi.ModifySpeed(facteurGel, dureeGel);
            }
        }

        // Les particules s'arrêtent pile au bord de la zone : distance = vitesse × durée de vie
        if (particlesFrost != null)
        {
            particlesFrost.SetActive(true);
            ParticleSystem ps = particlesFrost.GetComponent<ParticleSystem>();
            var main = ps.main;
            main.startSpeed = rayonGel / main.startLifetime.constant;
            ps.Play();
        }
    }

    // --- Flèche & sons ---

    private void UpdateFlecheOeil()
    {
        if (flecheOeil == null) return;

        if (!donthaveEye)
        {
            flecheOeil.Desactiver();
            return;
        }

        if (flecheOeil.cible == null)
        {
            GameObject oeil = GameObject.FindWithTag(tagOeil);
            if (oeil == null) return;
            flecheOeil.cible = oeil.transform;
        }

        flecheOeil.Activer();
    }

    private void UpdateSonEtat()
    {
        if (audioSource == null) return;
        if (etatInitialise && state == etatPrecedent) return;

        etatPrecedent = state;
        etatInitialise = true;

        audioSource.Stop();

        foreach (var s in sonsEtats)
        {
            if (s.etat != state || s.clip == null) continue;

            audioSource.clip = s.clip;
            audioSource.loop = s.boucle;
            audioSource.volume = s.volume;
            audioSource.Play();
            break;
        }
    }

    private void FixedUpdate()
    {
        Vector2 dir = new Vector2(Mathf.Round(move.x), Mathf.Round(move.y)).normalized;

        switch (state)
        {
            case playerstate.idle:
                rb.linearVelocity = dir * speed;
                break;
            case playerstate.confused:
                rb.linearVelocity = -dir * speed;
                break;
            case playerstate.invincible:
                rb.linearVelocity = (donthaveEye ? -dir : dir) * speed;
                break;
            case playerstate.stuck:
            case playerstate.prelaunch:
            case playerstate.launch:
                rb.linearVelocity = Vector2.zero;
                break;
        }
    }

    // --- Attaque (gel) ---

    public void AttackDebut(InputAction.CallbackContext context)
    {
        if (state == playerstate.invincible || state == playerstate.stuck) return;
        if (timerbarFrost < timeBarFrost) return; // jauge pas encore rechargée

        timerbarFrost = 0f;
        animatorClim.SetBool("Frost", true);
        Geler();
    }

    public void AttackFin(InputAction.CallbackContext context)
    {
        animatorClim.SetBool("Frost", false);
    }

    // --- Déguisement ---

    public void Deguiser(float duree)
    {
        if (deguisement != null) StopCoroutine(deguisement);
        deguisement = StartCoroutine(Deguisement(duree));
    }

    private IEnumerator Deguisement(float duree)
    {
        Show();
        yield return new WaitForSeconds(duree);
        Hide();
        deguisement = null;
    }

    public void Hide()
    {
        chapeau.SetActive(false);
        imper.SetActive(false);
        lunettes.SetActive(false);
    }

    public void Show()
    {
        chapeau.SetActive(true);
        imper.SetActive(true);
        lunettes.SetActive(true);
    }

    // --- Inputs ---

    private void OnEnable()
    {
        inputPlayer.actions.FindAction("Attack").started += AttackDebut;
        inputPlayer.actions.FindAction("Attack").canceled += AttackFin;
        inputPlayer.actions.FindAction("Jump").started += LancerOeil;
    }

    private void OnDisable()
    {
        inputPlayer.actions.FindAction("Attack").started -= AttackDebut;
        inputPlayer.actions.FindAction("Attack").canceled -= AttackFin;
        inputPlayer.actions.FindAction("Jump").started -= LancerOeil;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position - new Vector3(0, GetComponent<CapsuleCollider2D>().size.y / 2, 0), radiusDetectGround);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, rayonGel);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, rayonCaddie);
    }
}