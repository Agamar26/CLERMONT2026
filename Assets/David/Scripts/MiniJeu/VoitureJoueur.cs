using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class VoitureJoueur : MonoBehaviour
{
    public float vitesse = 6f;
    public float marge = 0.5f;   // dépassement autorisé au-delà des voies extrêmes

    private float yMin, yMax;

    void Start()
    {
        var voies = RaceManager.Instance.voies;
        yMin = Mathf.Min(voies) - marge;
        yMax = Mathf.Max(voies) + marge;
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        float input = 0f;

        var g = Gamepad.current;
        if (g != null)
        {
            input += g.leftStick.y.ReadValue();
            input += g.dpad.y.ReadValue();
        }

        var k = Keyboard.current;
        if (k != null)
        {
            if (k.upArrowKey.isPressed || k.wKey.isPressed) input += 1f;
            if (k.downArrowKey.isPressed || k.sKey.isPressed) input -= 1f;
        }

        input = Mathf.Clamp(input, -1f, 1f);

        var pos = transform.position;
        pos.y = Mathf.Clamp(pos.y + input * vitesse * Time.deltaTime, yMin, yMax);
        transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Voiture")) RaceManager.Instance.Perdu();
    }
}