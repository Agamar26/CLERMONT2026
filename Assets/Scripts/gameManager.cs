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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textInteraction.enabled = false;
        if (instance == null)
        {
            instance = this;
        }

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

    // Update is called once per frame
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

        var dds = Physics2D.OverlapCircle(doors.transform.position, radiusdetectplayer, layerPlayer);

        bool aDroite = player.instance.transform.position.x > doors.transform.position.x;
        bool dedans = magasinADroite ? aDroite : !aDroite;

        CourseOk = dffd == numberOfTasks;

        // Dehors : on peut toujours entrer. Dedans : il faut avoir toutes les courses.
        bool peutOuvrir = !dedans || CourseOk;
        doors.GetComponent<Animator>().SetBool("Open", dds && peutOuvrir);

        bool bloque = dds && dedans && !CourseOk;
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