using UnityEngine;
using System.Collections;

public class BonusCaddie : MonoBehaviour
{
    public float speedBonus = 1f;
    public float duree = 3f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        var p = player.instance;
        p.bonusTime = duree;
        p.bonusTimer = 0f;
        p.state = player.playerstate.invincible;
        Destroy(gameObject);
    }

   
}