using TMPro;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    public TextMeshProUGUI textInteraction;
    public float animatorSpeed = 1f;

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
