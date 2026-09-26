using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Collider2D limites;
    public float smoothSpeed = 0.125f;

    private Camera cam;
    private Vector3 speed;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 pos = new Vector3(target.position.x, target.position.y, transform.position.z);

        if (limites != null)
        {
            Bounds b = limites.bounds;
            float camHeight = cam.orthographicSize;
            float camWidth = camHeight * cam.aspect;

            pos.x = Borner(pos.x, b.min.x + camWidth, b.max.x - camWidth, b.center.x);
            pos.y = Borner(pos.y, b.min.y + camHeight, b.max.y - camHeight, b.center.y);
        }

        transform.position = Vector3.SmoothDamp(transform.position, pos, ref speed, smoothSpeed);
    }

    private float Borner(float v, float min, float max, float center)
    {
        return min > max ? center : Mathf.Clamp(v, min, max);
    }
}