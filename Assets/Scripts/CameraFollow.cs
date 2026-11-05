using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform target;

    [Header("Suavizado")]
    [SerializeField] private float smoothTime = 0.15f;

    [Header("Límites de la arena")]
    [SerializeField] private Collider2D arenaBounds;   // BoxCollider2D que cubre la arena (opcional)
    [SerializeField] private bool useManualBounds;     // solo si no usas arenaBounds
    [SerializeField] private float manualMinX, manualMaxX, manualMinY, manualMaxY;

    private Camera cam;
    private Vector3 velocity;
    private bool boundsReady;
    private float minX, maxX, minY, maxY;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        if (target == null)
        {
            PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
            if (player != null) target = player.transform;
        }

        if (arenaBounds != null)
        {
            Bounds b = arenaBounds.bounds; // límites en coordenadas del mundo
            minX = b.min.x; maxX = b.max.x;
            minY = b.min.y; maxY = b.max.y;
            boundsReady = true;
        }
        else if (useManualBounds)
        {
            minX = manualMinX; maxX = manualMaxX;
            minY = manualMinY; maxY = manualMaxY;
            boundsReady = true;
        }
        // Si no hay ninguno de los dos, la cámara sigue al jugador sin límites

        // Arranca ya centrada en el jugador (así la intro no muestra un desplazamiento)
        if (target != null)
        {
            Vector3 start = new Vector3(target.position.x, target.position.y, transform.position.z);
            transform.position = ClampToBounds(start);
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = new Vector3(target.position.x, target.position.y, transform.position.z);
        Vector3 smoothed = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);

        transform.position = ClampToBounds(smoothed);
    }

    private Vector3 ClampToBounds(Vector3 pos)
    {
        if (!boundsReady) return pos;

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        float clampMinX = minX + halfWidth;
        float clampMaxX = maxX - halfWidth;
        float clampMinY = minY + halfHeight;
        float clampMaxY = maxY - halfHeight;

        // Si la arena es más chica que la vista, la cámara se queda centrada
        pos.x = clampMinX > clampMaxX ? (minX + maxX) / 2f : Mathf.Clamp(pos.x, clampMinX, clampMaxX);
        pos.y = clampMinY > clampMaxY ? (minY + maxY) / 2f : Mathf.Clamp(pos.y, clampMinY, clampMaxY);
        return pos;
    }
}