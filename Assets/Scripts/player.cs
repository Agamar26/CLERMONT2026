using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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

    [Header("Vie")]
    public float rayonContact = 0.5f;
    public float dureeInvulnerabilite = 1f;
    public Color couleurDegats = new Color(1f, 0.3f, 0.3f);
    public float frequenceClignotement = 10f;
    public System.Action OnMort;

    [Header("Gel de zone")]
    public float rayonGel = 3f;
    public float dureeGel = 2f;
    public float degatsGel = 10f;
    public float distancePoussee = 1.5f;
    public float dureePoussee = 0.25f;

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

    [Header("Sons d'actions (source séparée : pas coupés par les changements d'état)")]
    public AudioSource audioActions;      // créée automatiquement si vide
    public AudioClip sonGel;
    public AudioClip sonLancer;
    public AudioClip[] sonsDegats;        // tiré au hasard à chaque coup
    public AudioClip sonMort;             // si vide : un son de dégâts au coup fatal
    [Range(0f, 1f)] public float volumeActions = 1f;
    [Range(0f, 0.3f)] public float variationPitch = 0.08f;

    public bool EstMort => health <= 0f;
    public bool EstDeguise { get; private set; }

    private Coroutine deguisement;
    private playerstate etatPrecedent;
    private bool etatInitialise;
    private Vector2 derniereDirection = Vector2.right;
    private float invulTimer;
    private SpriteRenderer[] sprites;
    private Color[] couleursOrigine;
    private bool clignoteRouge;
    private int dernierSonDegats = -1;

    void Start()
    {
        Time.timeScale = 1f;
        if (instance == null) instance = this;

        if (audioSource != null) audioSource.playOnAwake = false;
        if (audioActions == null) audioActions = gameObject.AddComponent<AudioSource>();
        audioActions.playOnAwake = false;

        if (flecheOeil != null && flecheOeil.joueur == null) flecheOeil.joueur = transform;

        sprites = GetComponentsInChildren<SpriteRenderer>(true);
        couleursOrigine = new Color[sprites.Length];
        for (int i = 0; i < sprites.Length; i++) couleursOrigine[i] = sprites[i].color;
    }

    void Update()
    {
        if (instance == null) instance = this;

        if (invulTimer > 0f) invulTimer -= Time.deltaTime;
        VerifierContacts();

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

    // Après l'Animator : la teinte n'est pas écrasée par les anims
    void LateUpdate()
    {
        UpdateClignotement();
    }

    // --- Vie ---

    private void VerifierContacts()
    {
        // Déguisé : pas reconnu, donc pas de dégâts
        if (EstMort || EstDeguise || invulTimer > 0f || state == playerstate.invincible) return;

        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, rayonContact))
        {
            EnnemiChase ennemi = col.GetComponentInParent<EnnemiChase>();
            if (ennemi != null && ennemi.PeutBlesser)
            {
                TakeDamage(ennemi.DegatsContact);
                return; // un seul coup par contact, l'invulnérabilité gère la suite
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (EstMort || amount <= 0f || invulTimer > 0f || state == playerstate.invincible) return;

        health = Mathf.Max(health - amount, 0f);
        invulTimer = dureeInvulnerabilite;
        Debug.Log($"Joueur touché : -{amount} → {health}/{maxHealth}");

        if (EstMort)
        {
            JouerSon(sonMort != null ? sonMort : SonDegatsAuHasard());
            Mourir();
        }
        else
        {
            JouerSon(SonDegatsAuHasard());
            animatorClim.SetTrigger("Hurt");
        }
    }

    // Tirage au hasard, sans répéter deux fois de suite le même son
    private AudioClip SonDegatsAuHasard()
    {
        if (sonsDegats == null || sonsDegats.Length == 0) return null;
        if (sonsDegats.Length == 1) return sonsDegats[0];

        int i = Random.Range(0, sonsDegats.Length - 1);
        if (i >= dernierSonDegats && dernierSonDegats >= 0) i++; // saute le dernier joué
        dernierSonDegats = i;
        return sonsDegats[i];
    }

    public void Heal(float amount)
    {
        if (EstMort || amount <= 0f) return;
        health = Mathf.Min(health + amount, maxHealth);
    }

    private void Mourir()
    {
        state = playerstate.stuck;
        rb.linearVelocity = Vector2.zero;
        animatorClim.SetTrigger("Death");
        OnMort?.Invoke();
    }

    private void UpdateClignotement()
    {
        if (invulTimer > 0f)
        {
            bool rouge = Mathf.Repeat(invulTimer * frequenceClignotement, 1f) > 0.5f;
            for (int i = 0; i < sprites.Length; i++)
                sprites[i].color = rouge ? couleurDegats : couleursOrigine[i];
            clignoteRouge = true;
        }
        else if (clignoteRouge)
        {
            for (int i = 0; i < sprites.Length; i++)
                sprites[i].color = couleursOrigine[i];
            clignoteRouge = false;
        }
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
        JouerSon(sonLancer);
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

    // --- Gel de zone : gèle, blesse et repousse, sans bloquer le déplacement ---

    public void LancerGel(InputAction.CallbackContext context)
    {
        if (state != playerstate.idle && state != playerstate.confused) return;
        if (timerbarFrost < timeBarFrost) return; // jauge pas encore rechargée

        timerbarFrost = 0f;
        JouerSon(sonGel);
        Geler();
    }

    private void Geler()
    {
        var touches = new HashSet<EnnemiChase>();

        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, rayonGel))
        {
            EnnemiChase ennemi = col.GetComponentInParent<EnnemiChase>();
            if (ennemi == null || !touches.Add(ennemi)) continue;

            ennemi.Geler(dureeGel);

            EnemyStats stats = ennemi.GetComponentInParent<EnemyStats>();
            if (stats != null && !stats.IsDead)
            {
                stats.TakeDamage(degatsGel);
                if (stats == null || stats.IsDead) continue; // mort sur le coup : pas de poussée
            }

            Vector2 dir = ennemi.transform.position - transform.position;
            if (dir.sqrMagnitude < 0.0001f) dir = derniereDirection; // pile sur le joueur
            StartCoroutine(Repousser(ennemi.transform, dir.normalized));
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

    // Poussée en ease-out ; via le NavMeshAgent si présent pour ne pas traverser les murs
    private IEnumerator Repousser(Transform cible, Vector2 dir)
    {
        NavMeshAgent agent = cible.GetComponent<NavMeshAgent>();
        Rigidbody2D corps = cible.GetComponent<Rigidbody2D>();
        float t = 0f, parcouru = 0f;

        while (t < dureePoussee)
        {
            if (cible == null) yield break; // détruit entre-temps

            t += Time.deltaTime;
            float k = 1f - Mathf.Pow(1f - Mathf.Clamp01(t / dureePoussee), 2f);
            float pas = k * distancePoussee - parcouru;
            parcouru += pas;
            Vector2 delta = dir * pas;

            if (agent != null && agent.enabled && agent.isOnNavMesh) agent.Move(delta);
            else if (corps != null) corps.position += delta;
            else cible.position += (Vector3)delta;

            yield return null;
        }
    }

    // --- Flèche & sons ---

    // Son ponctuel : se superpose aux autres, pitch légèrement aléatoire pour éviter la répétition
    public void JouerSon(AudioClip clip)
    {
        if (clip == null || audioActions == null) return;

        audioActions.pitch = 1f + Random.Range(-variationPitch, variationPitch);
        audioActions.PlayOneShot(clip, volumeActions);
    }

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
        EstDeguise = false;
    }

    public void Show()
    {
        chapeau.SetActive(true);
        imper.SetActive(true);
        lunettes.SetActive(true);
        EstDeguise = true;
    }

    // --- Inputs ---

    private void OnEnable()
    {
        inputPlayer.actions.FindAction("Attack").started += LancerGel;
        inputPlayer.actions.FindAction("Jump").started += LancerOeil;
    }

    private void OnDisable()
    {
        inputPlayer.actions.FindAction("Attack").started -= LancerGel;
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

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rayonContact);
    }
}