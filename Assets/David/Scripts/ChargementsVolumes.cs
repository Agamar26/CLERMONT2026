using UnityEngine;
using UnityEngine.Audio;

public class ChargementsVolumes : MonoBehaviour
{
   public AudioMixer mixer;
   public string[] parametres = { "MasterVolume", "MusicVolume", "EffetsVolume" };

    void Start()
    {
        foreach (var parametre in parametres)
        {
            float value = PlayerPrefs.GetFloat(parametre, 1f); 
            VolumeSlider.Regler(mixer, parametre, value);
        }
    }

}
