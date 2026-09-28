using UnityEngine;
using System.Collections.Generic;

public class DecorEtageres : MonoBehaviour
{
    public Sprite[] sprites;
    public SpriteRenderer[] etageres;

    void Awake()
    {
        if (sprites.Length == 0) return;

        var pioche = new List<Sprite>();

        foreach (var etagere in etageres)
        {
            if (pioche.Count == 0)
            {
                pioche.AddRange(sprites);
                for (int i = pioche.Count - 1; i > 0; i--)
                {
                    int j = Random.Range(0, i + 1);
                    (pioche[i], pioche[j]) = (pioche[j], pioche[i]);
                }
            }

            etagere.sprite = pioche[pioche.Count - 1];
            pioche.RemoveAt(pioche.Count - 1);
        }
    }
}