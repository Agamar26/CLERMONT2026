using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnnemiChase : MonoBehaviour
{
    private NavMeshAgent agent;
    private SpriteRenderer sr;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        sr = GetComponent<SpriteRenderer>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        transform.rotation = Quaternion.identity;
        agent.enabled = true;
    }

    void Update()
    {
        if (player.instance == null) return;

        agent.SetDestination(player.instance.transform.position);

        if (sr != null && Mathf.Abs(agent.velocity.x) > 0.01f)
            sr.flipX = agent.velocity.x < 0;
    }
}