using UnityEngine;

[CreateAssetMenu(fileName = "LokiClone", menuName = "Game/Abilities/Loki Clone")]
public class LokiCloneAbility : AbilityLogic
{
    [Header("Opciones Específicas del Clon")]
    public GameObject clonePrefab; // Un prefab estático o con animación simple
    public float cloneDuration = 5f;

    public override void Activate(Transform playerTransform)
    {
        if (clonePrefab != null)
        {
            // 1. Instanciamos el clon donde está el jugador
            GameObject clone = Instantiate(clonePrefab, playerTransform.position, Quaternion.identity);

            // 2. Buscamos a todos los enemigos activos en la arena
            EnemyAI[] enemies = FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
            foreach (EnemyAI enemy in enemies)
            {
                // Sobrescribimos su objetivo temporalmente
                enemy.target = clone.transform;
            }

            // 3. El clon desaparece tras unos segundos
            Destroy(clone, cloneDuration);

            /* Nota: Para que el aggro regrese al jugador al destruirse el clon, 
               deberás añadirle un script pequeñito al 'clonePrefab' que, en su OnDestroy(), 
               vuelva a hacer un FindObjectsByType y regrese el target al "Player". */
        }
    }
}