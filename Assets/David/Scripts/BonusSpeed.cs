using UnityEngine;

public class BonusSpeed : MonoBehaviour
{

    public float speedBonus=1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            player.instance.speed += speedBonus;
            Destroy(gameObject);
        }
    }
}
