using UnityEngine;

public class EffetCoeurs : MonoBehaviour
{
    public ParticleSystem coeurs;

    // Appelée par l'Animation Event
    public void JouerCoeurs()
    {
        coeurs.Play();
    }
}