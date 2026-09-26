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
    public string debutmessagemission;
    public int numberOfTasks;
    public GameObject prefMissionUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textInteraction.enabled = false;
        if (instance == null)
        {
            instance = this;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
}
