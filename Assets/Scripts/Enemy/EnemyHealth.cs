using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Salud")]
    public int maxHealth = 30;
    public bool isBoss = false;

    [Header("Efectos de Impacto")]
    public float stunDuration = 0.2f;
    public float knockbackForce = 5f;

    private int currentHealth;
    private ArcadeGameLoop gameLoop;
    private EnemyAI ai;

    void Start()
    {
        currentHealth = maxHealth;
        gameLoop = FindFirstObjectByType<ArcadeGameLoop>();
        ai = GetComponent<EnemyAI>();
    }

    public void TakeDamage(int damageAmount, Vector2 hitDirection)
    {
        if (ai != null && ai.currentState == EnemyAI.EnemyState.Spawning) return;

        currentHealth -= damageAmount;

        if (currentHealth <= 0) Die();
        else
        {
            if (ai != null) ai.ApplyStun(stunDuration,hitDirection * knockbackForce);
        }
    }

    public void Die()
    {
        if (gameLoop != null) gameLoop.RegisterKill(isBoss);
        Destroy(gameObject);
    }
}
