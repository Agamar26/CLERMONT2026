using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    public string sceneJeu = "Game";

    public GameObject panelMenu;
    public GameObject panelOptions;
    public GameObject panelCredits;

    [Header("Premier élément sélectionné par panneau")]
    public GameObject premierMenu;     // bouton Jouer
    public GameObject premierOptions;  // premier slider ou bouton Retour
    public GameObject premierCredits;  // bouton Retour

    private GameObject selectionParDefaut;

    void Start()
    {
        Afficher(panelMenu, premierMenu);
    }

    void Update()
    {
        // Si un clic souris a désélectionné, on revient sur le bouton par défaut
        if (EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(selectionParDefaut);
    }

    public void Jouer()
    {
        PlayerPrefs.Save();
        SceneManager.LoadScene(sceneJeu);
    }

    public void Options() => Afficher(panelOptions, premierOptions);
    public void Credits() => Afficher(panelCredits, premierCredits);

    public void Retour()
    {
        PlayerPrefs.Save();
        Afficher(panelMenu, premierMenu);
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

    private void Afficher(GameObject panel, GameObject premier)
    {
        panelMenu.SetActive(panel == panelMenu);
        if (panelOptions) panelOptions.SetActive(panel == panelOptions);
        if (panelCredits) panelCredits.SetActive(panel == panelCredits);

        selectionParDefaut = premier;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(premier);
    }
}