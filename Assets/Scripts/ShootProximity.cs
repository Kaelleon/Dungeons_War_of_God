using UnityEngine;

public class ShootProximity : ShooterBase
{
    [Header("Detección")]
    [SerializeField] private float detectionRadius = 6f;
    [SerializeField] private LayerMask enemyLayer;

    private Transform currentTarget;

    private void Update()
    {
        currentTarget = FindClosestEnemy();

        if (currentTarget != null)
        {
            Vector2 dir = (currentTarget.position - firePoint.position).normalized;
            Shoot(dir);
        }
    }

    private Transform FindClosestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);

        Transform closest = null;
        float minDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            float dist = ((Vector2)hit.transform.position - (Vector2)transform.position).sqrMagnitude;
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = hit.transform;
            }
        }
        return closest;
    }

    // Dibuja el área de detección en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}