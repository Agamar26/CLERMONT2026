using UnityEngine;

public class enemy : MonoBehaviour
{
    public float speed;
    public enum enemyState { idle,chase,search,confused};
    public enemyState state;
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
