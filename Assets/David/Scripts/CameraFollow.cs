using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Collider2D[] zones;          // extérieur, magasin...
    public float smoothSpeed = 0.125f;

    private Camera cam;
    private Vector3 speed;
    private Collider2D zoneActuelle;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        // Place directement la caméra au démarrage, sans glissement
        if (target == null) return;
        MettreAJourZone();
        transform.position = Cible();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        MettreAJourZone();
        transform.position = Vector3.SmoothDamp(transform.position, Cible(), ref speed, smoothSpeed);
    }

    private void MettreAJourZone()
    {
        foreach (var z in zones)
        {
            if (z != null && z.OverlapPoint(target.position))
            {
                zoneActuelle = z;
                return;
            }
        }
        // Aucune zone trouvée : on garde la précédente
    }

    private Vector3 Cible()
    {
        Vector3 pos = new Vector3(target.position.x, target.position.y, transform.position.z);

        if (zoneActuelle != null)
        {
            Bounds b = zoneActuelle.bounds;
            float camHeight = cam.orthographicSize;
            float camWidth = camHeight * cam.aspect;

            pos.x = Borner(pos.x, b.min.x + camWidth, b.max.x - camWidth, b.center.x);
            pos.y = Borner(pos.y, b.min.y + camHeight, b.max.y - camHeight, b.center.y);
        }

        return pos;
    }

    private float Borner(float v, float min, float max, float center)
    {
        return min > max ? center : Mathf.Clamp(v, min, max);
    }
}