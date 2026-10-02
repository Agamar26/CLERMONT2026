using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CreditsScroller : MonoBehaviour
{
    [SerializeField] private RectTransform viewport;
    [SerializeField] private RectTransform content;
    [SerializeField] private float speed = 60f;
    [SerializeField] private float fastMultiplier = 4f;
    [SerializeField] private bool loop = false;
    [SerializeField] private UnityEvent onFinished;

    private float startY;
    private float endY;
    private bool finished;

    private void OnEnable()
    {
        Restart();
    }

    public void Restart()
    {
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);

        startY = -viewport.rect.height;   // le haut du contenu démarre sous la zone visible
        endY = content.rect.height;       // le bas du contenu sort par le haut

        content.anchoredPosition = new Vector2(content.anchoredPosition.x, startY);
        finished = false;
    }

    private void Update()
    {
        if (finished) return;

        float s = speed * (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space) ? fastMultiplier : 1f);

        // unscaledDeltaTime : fonctionne même si Time.timeScale = 0 dans le menu
        Vector2 pos = content.anchoredPosition;
        pos.y += s * Time.unscaledDeltaTime;
        content.anchoredPosition = pos;

        if (pos.y >= endY)
        {
            if (loop)
            {
                Restart();
            }
            else
            {
                finished = true;
                onFinished?.Invoke();   // ex : fermer le panel / revenir au menu
            }
        }
    }
}