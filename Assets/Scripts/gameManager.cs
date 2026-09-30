using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    public TextMeshProUGUI textInteraction;
    public float animatorSpeed = 1f;
    public List<string> objCourses = new List<string>();
    public List<string> objCoursesToGet = new List<string>();
    public List<bool> objCoursesToGetTaken = new List<bool>();
    public List<GameObject> objCoursesToGetTakenUI = new List<GameObject>();
    public string debutmessagemission;
    public int numberOfTasks;
    public GameObject prefMissionUI;
    public GameObject containeruitasks;
    public LayerMask layerPlayer;
    public float radiusdetectplayer;
    public GameObject doors;
    public Image FrostBar;
    public bool CourseOk = false;
    public bool magasinADroite = true;   // true si l'intérieur du magasin est à droite de la porte
    public float delaiFermeture = 0.5f;  // la porte reste ouverte ce temps après le passage

    private bool dedansMemorise;         // côté d'où le joueur arrive, figé tant qu'il est dans la zone
    private float timerFermeture;
    private Animator doorAnimator;

    void Start()
    {
        textInteraction.enabled = false;
        if (instance == null)
        {
            instance = this;
        }

        doorAnimator = doors.GetComponent<Animator>();

        // Évite une boucle infinie si on demande plus d'articles qu'il n'en existe
        if (numberOfTasks > objCourses.Count)
        {
            Debug.LogWarning($"numberOfTasks ({numberOfTasks}) > nombre d'articles ({objCourses.Count}), valeur plafonnée.");
            numberOfTasks = objCourses.Count;
        }

        for (int i = 0; i < numberOfTasks; i++)
        {
            var zae = Random.Range(0, objCourses.Count);
            while (objCoursesToGet.Contains(objCourses[zae]))
            {
                zae = Random.Range(0, objCourses.Count);
            }
            objCoursesToGet.Add(objCourses[zae]);
            objCoursesToGetTaken.Add(false);
            var ezez = Instantiate(prefMissionUI, containeruitasks.transform);
            ezez.GetComponentInChildren<TextMeshProUGUI>().text = $"{debutmessagemission} {objCourses[zae]}";
            objCoursesToGetTakenUI.Add(ezez);
        }
    }

    void Update()
    {
        if (instance == null)
        {
            instance = this;
        }
        int dffd = 0;
        for (int i = 0; i < objCoursesToGetTaken.Count; i++)
        {
            if (objCoursesToGetTaken[i])
            {
                dffd++;
                objCoursesToGetTakenUI[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
            }
            else
            {
                objCoursesToGetTakenUI[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
            }
        }

        CourseOk = dffd == numberOfTasks;

        bool dds = Physics2D.OverlapCircle(doors.transform.position, radiusdetectplayer, layerPlayer);

        bool aDroite = player.instance.transform.position.x > doors.transform.position.x;
        bool dedansActuel = magasinADroite ? aDroite : !aDroite;

        // Hors zone : on suit le côté réel.
        // Dans la zone : on garde le côté d'arrivée, pour ne pas refermer la porte en plein passage.
        if (!dds) dedansMemorise = dedansActuel;

        // Dehors : on peut toujours entrer. Dedans : il faut avoir toutes les courses.
        bool peutOuvrir = !dedansMemorise || CourseOk;

        if (dds && peutOuvrir) timerFermeture = delaiFermeture;
        else timerFermeture -= Time.deltaTime;

        doorAnimator.SetBool("Open", timerFermeture > 0f);

        bool bloque = dds && dedansMemorise && !CourseOk;
        textInteraction.enabled = bloque;
        if (bloque) textInteraction.text = "Recuperez toutes les courses avant de sortir";

        FrostBar.fillAmount = player.instance.timerbarFrost / player.instance.timeBarFrost;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(doors.transform.position, radiusdetectplayer);
    }
}