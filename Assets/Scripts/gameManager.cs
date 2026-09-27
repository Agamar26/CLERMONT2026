using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textInteraction.enabled = false;
        if (instance == null)
        {
            instance = this;
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
        if(instance == null)
        {
            instance = this;
        }
        int dffd = 0;
        for(int i = 0;i < objCoursesToGetTaken.Count;i++)
        {
            if(objCoursesToGetTaken[i])
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
        if (dffd == numberOfTasks)
        {
           
            if(dds)
            {
                doors.GetComponent<Animator>().SetBool("Open", true);
            }
            else
            {
                doors.GetComponent<Animator>().SetBool("Open", false);
            }
        }
        else
        {
            if(dds)
            {
                textInteraction.GetComponent<TextMeshProUGUI>().enabled = true;
                textInteraction.GetComponent<TextMeshProUGUI>().text = "Recuperez toutes les courses avant de sortir";
            }
            else
            {
                textInteraction.GetComponent<TextMeshProUGUI>().enabled = false;
            }
            
        }
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(doors.transform.position, radiusdetectplayer);
    }
}
