using UnityEngine;

public class BonusSoin : MonoBehaviour
{
   public float soinBonus = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        
       var p = player.instance;

       if (p.health >= p.maxHealth) return;

       p.health = Mathf.Min(p.health + soinBonus, p.maxHealth);
        Destroy(gameObject);
                
    }
}
