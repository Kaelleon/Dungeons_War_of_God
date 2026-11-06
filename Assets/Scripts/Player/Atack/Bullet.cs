using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifeTime = 3f;

    private int currentDamage = 1;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(int damageAmount)
    {
        currentDamage = damageAmount;
    }

    public void Launch(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * speed; // En Unity 2022 o anterior usa rb.velocity

        // Rota la bala para que mire hacia donde va
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Vector2 hitDirection = rb.linearVelocity.normalized;

            // 1. Intenta dañar a un enemigo normal
            EnemyHealth normalEnemy = other.GetComponent<EnemyHealth>();
            if (normalEnemy != null)
            {
                normalEnemy.TakeDamage(currentDamage, hitDirection);
            }
            else
            {
                // 2. Si no es un enemigo normal, intenta dañar al Jefe
                BossHealth boss = other.GetComponent<BossHealth>();
                if (boss != null)
                {
                    boss.TakeDamage(currentDamage, hitDirection);
                }
            }

            Destroy(gameObject);
        }
    }
}