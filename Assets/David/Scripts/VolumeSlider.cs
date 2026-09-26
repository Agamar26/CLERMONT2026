using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour
{

    public AudioMixer mixer;
    public string parametre = "MasterVolume";
    Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
        slider.minValue = 0.0001f; // Valeur minimale pour éviter le logarithme de zéro
        slider.maxValue = 1f; // Valeur maximale pour le volume
    }

    void Start()
    {
       slider.SetValueWithoutNotify(PlayerPrefs.GetFloat(parametre, 1f)); // Récupère la valeur sauvegardée ou 1 par défaut

        slider.onValueChanged.AddListener(Appliquer);
    }

    void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(Appliquer);
    }

   private void Appliquer(float value)
    {
       Regler(mixer,parametre, value);
        PlayerPrefs.SetFloat(parametre, value); // Sauvegarde la valeur du volume
    }

    public static void Regler(AudioMixer mixer, string parametre, float value)
    {
        // Convertir la valeur du slider (0 à 1) en décibels
        float dB = Mathf.Log10(value) * 20f;
        mixer.SetFloat(parametre, dB);
    }
}