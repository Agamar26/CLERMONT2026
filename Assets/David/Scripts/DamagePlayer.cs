using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    public int damageAmount = 10;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            var playerInstance = player.instance;
            playerInstance.health -= damageAmount;
        }
    }
}
