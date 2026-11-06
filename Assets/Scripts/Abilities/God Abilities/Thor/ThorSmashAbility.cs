using UnityEngine;

[CreateAssetMenu(fileName = "ThorSmash", menuName = "Game/Abilities/Thor Smash")]
public class ThorSmashAbility : AbilityLogic
{
    [Header("Impacto en Área (AoE)")]
    public int damage = 50;
    public float radius = 4f;
    public float knockbackMultiplier = 2f;
    public GameObject impactEffectPrefab; // Para poner partículas o un cráter

    public override void Activate(Transform playerTransform)
    {
        // 1. Efecto visual (Opcional)
        if (impactEffectPrefab != null)
        {
            GameObject fx = Instantiate(impactEffectPrefab, playerTransform.position, Quaternion.identity);
            Destroy(fx, 1f); // Destruye las partículas después de 1 segundo
        }

        // 2. Crear un círculo invisible para detectar a todos los enemigos en el área
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(playerTransform.position, radius);

        foreach (Collider2D hit in hitEnemies)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    // 3. Calcular la dirección del empuje (desde el jugador hacia el enemigo)
                    Vector2 pushDirection = (hit.transform.position - playerTransform.position).normalized;

                    // 4. Aplicar el daño y el empuje multiplicando la fuerza
                    enemyHealth.TakeDamage(damage, pushDirection * knockbackMultiplier);
                }
            }
        }
    }
}