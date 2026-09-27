using UnityEngine;
using UnityEngine.AI;

public class particleScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnParticleCollision(GameObject other)
    {
        if(other.gameObject.tag == "Enemy" && other.GetComponent<NavMeshAgent>().speed ==  other.GetComponent<EnemyStats>().speeddebase)
        {
            other.GetComponent<EnemyStats>().ModifySpeed(0.5f, 3);
        }
    }
}
