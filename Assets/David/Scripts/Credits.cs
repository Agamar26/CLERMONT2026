using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    public RectTransform contenu;         // anchor et pivot en haut-centre
    public float vitesse = 80f;           // pixels par seconde
    public float multiplicateurAccel = 4f;
    public string sceneSuivante = "Menu";

    void Start()
    {
        Canvas.ForceUpdateCanvases();     // la hauteur du texte est calculée avant de placer le contenu

        // Départ : le haut du texte pile sous le bas de l'écran
        float hauteurEcran = ((RectTransform)contenu.parent).rect.height;
        contenu.anchoredPosition = new Vector2(0f, -hauteurEcran);
    }

    void Update()
    {
        var clavier = Keyboard.current;
        var manette = Gamepad.current;

        bool passer = (clavier?.escapeKey.wasPressedThisFrame ?? false) || (manette?.startButton.wasPressedThisFrame ?? false);
        if (passer)
        {
            Quitter();
            return;
        }

        bool accel = (clavier?.spaceKey.isPressed ?? false) || (manette?.buttonSouth.isPressed ?? false);
        contenu.anchoredPosition += Vector2.up * vitesse * (accel ? multiplicateurAccel : 1f) * Time.deltaTime;

        // Fin : le bas du texte est sorti par le haut
        if (contenu.anchoredPosition.y > contenu.rect.height) Quitter();
    }

    private void Quitter()
    {
        SceneManager.LoadScene(sceneSuivante);
    }
}