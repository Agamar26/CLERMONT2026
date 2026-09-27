using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class BarreProgression : MonoBehaviour
{
    private Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.interactable = false;
    }

    void Update()
    {
        if (RaceManager.Instance == null) return;
        slider.value = RaceManager.Instance.Progression;
    }
}