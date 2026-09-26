using UnityEngine;

public class enemy : MonoBehaviour
{
    public float speed;
    public enum enemyState { idle,chase,search,confused};
    public enemyState state;
    public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case enemyState.idle:
                if(animator != null)
                {
                    animator.speed = 1f * gameManager.instance.animatorSpeed;
                }
                transform.position = Vector3.MoveTowards(transform.position,player.instance.transform.position,speed * gameManager.instance.animatorSpeed * Time.deltaTime);
                break;

                case enemyState.chase:

                break;

                case enemyState.search:

                break;

            case enemyState.confused:

                break;
        }
    }
}
