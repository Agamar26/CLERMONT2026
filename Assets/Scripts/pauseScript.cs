using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class pauseScript : MonoBehaviour
{
    public GameObject panelPause;
    public GameObject premierBouton;          // bouton Reprendre
    public string sceneMenu = "MainMenu";

    public bool EnPause { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool touche =
           (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) ||
           (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame);

        if (touche)
        {
            if (EnPause) Reprendre();
            else Pause();
        }

        // Garde un bouton sélectionné pour la navigation clavier/manette
        if (EnPause && EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(premierBouton);
    }
    public void Pause()
    {
        EnPause = true;
       
        panelPause.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(premierBouton);
    }
    public void changescene()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(sceneMenu);
    }
    public void Reprendre()
    {
        EnPause = false;
        panelPause.SetActive(false);

    }
}
