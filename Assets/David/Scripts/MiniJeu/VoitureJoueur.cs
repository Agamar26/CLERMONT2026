using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class VoitureJoueur : MonoBehaviour
{
    public float vitesseChangement = 10f;

    private int voie;

    void Start()
    {
        var voies = RaceManager.Instance.voies;
        voie = voies.Length / 2;
        var p = transform.position;
        p.y = voies[voie];
        transform.position = p;
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        var k = Keyboard.current;
        var g = Gamepad.current;

        bool haut = (k != null && (k.upArrowKey.wasPressedThisFrame || k.wKey.wasPressedThisFrame))
                 || (g != null && g.dpad.up.wasPressedThisFrame);
        bool bas = (k != null && (k.downArrowKey.wasPressedThisFrame || k.sKey.wasPressedThisFrame))
                 || (g != null && g.dpad.down.wasPressedThisFrame);

        var voies = RaceManager.Instance.voies;
        if (haut) voie = Mathf.Min(voie + 1, voies.Length - 1);
        if (bas) voie = Mathf.Max(voie - 1, 0);

        var pos = transform.position;
        pos.y = Mathf.MoveTowards(pos.y, voies[voie], vitesseChangement * Time.deltaTime);
        transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Voiture")) RaceManager.Instance.Perdu();
    }
}