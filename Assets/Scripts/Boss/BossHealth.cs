using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Salud del Jefe")]
    public int maxHealth = 1500;

    [Header("Recompensas")]
    public GameObject rewardChestPrefab; // El cofre que aparecerá al morir

    private int currentHealth;
    private ArcadeGameLoop gameLoop;

    private void Start()
    {
        currentHealth = maxHealth;
        gameLoop = FindFirstObjectByType<ArcadeGameLoop>();
    }

    public void TakeDamage(int damageAmount, Vector2 hitDirection)
    {
        currentHealth -= damageAmount;

        // A diferencia de EnemyHealth, omitimos el Stun y el empuje para darle peso al Jefe

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // 1. Instanciar el cofre de recompensa en la posición exacta donde murió el Jefe[cite: 184]
        if (rewardChestPrefab != null)
        {
            Instantiate(rewardChestPrefab, transform.position, Quaternion.identity);
        }

        // 2. Avisar al GameLoop (pasando 'true' para indicar que es un Jefe y sumar 5000 puntos)[cite: 237]
        if (gameLoop != null)
        {
            gameLoop.RegisterKill(true);
        }

        Destroy(gameObject);
    }
}