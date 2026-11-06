using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Salud del Jefe")]
    public int maxHealth = 1500;

    [Header("Recompensas")]
    public GameObject rewardChestPrefab; // El cofre que aparecerá al morir

    private int currentHealth;
    private ArcadeGameLoop gameLoop;

    void Start()
    {
        currentHealth = maxHealth;
        gameLoop = FindFirstObjectByType<ArcadeGameLoop>();
    }

    public void TakeDamage(int damageAmount, Vector2 hitDirection)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (rewardChestPrefab != null)
        {
            Instantiate(rewardChestPrefab, transform.position, Quaternion.identity);
        }

        if (gameLoop != null)
            gameLoop.RegisterKill(true);
        
        Destroy(gameObject);
    }
}
