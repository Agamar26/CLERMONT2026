using UnityEngine;
using System.Collections;

public class BonusSpeed : MonoBehaviour
{
    public float speedBonus = 1f;
    public float duree = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        var p = player.instance;
        p.StartCoroutine(ApplySpeedBonus(p, speedBonus, duree));
        Destroy(gameObject);
    }

    private static IEnumerator ApplySpeedBonus(player p, float bonus, float duree)
    {
        p.speed += bonus;
        yield return new WaitForSeconds(duree);
        p.speed -= bonus;
    }
}