using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuPause : MonoBehaviour
{
    public GameObject panelPause;
    public GameObject premierBouton;          // bouton Reprendre
    public string sceneMenu = "MainMenu";

    public static bool EnPause { get; private set; }

    void Start()
    {
        Reprendre();
    }

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
        Time.timeScale = 0f;
        AudioListener.pause = true;
        panelPause.SetActive(true);
        player.instance.state = player.playerstate.stuck;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(premierBouton);
    }

    public void Reprendre()
    {
        EnPause = false;        
        Time.timeScale = 1f;
        AudioListener.pause = false;
        panelPause.SetActive(false);
        player.instance.state = player.playerstate.idle;
    }

    public void RetourMenu()
    {
        Reprendre();   // remet timeScale à 1 avant de changer de scène
        Fondu.Charger(sceneMenu);
    }

    public void Quitter()
    {
        PlayerPrefs.Save();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}