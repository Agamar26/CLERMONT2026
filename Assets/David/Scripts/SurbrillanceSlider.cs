using UnityEngine;
using UnityEngine.EventSystems;

public class SurbrillanceSlider : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public GameObject surbrillance;   // l'image rose derrière la ligne

    void OnEnable()
    {
        surbrillance.SetActive(EventSystem.current != null &&
                               EventSystem.current.currentSelectedGameObject == gameObject);
    }

    public void OnSelect(BaseEventData eventData) => surbrillance.SetActive(true);
    public void OnDeselect(BaseEventData eventData) => surbrillance.SetActive(false);
}