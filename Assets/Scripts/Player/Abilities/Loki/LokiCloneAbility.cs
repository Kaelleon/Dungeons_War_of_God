using UnityEngine;

[CreateAssetMenu(fileName = "LokiClone", menuName = "Game/Abilities/Loki Clone")]
public class LokiCloneAbility : AbilityLogic
{
    [Header("Opciones Específicas del Clon")]
    public GameObject clonePrefab; 
    public float cloneDuration = 5f;

    public override void Activate(Transform playerTransform)
    {
        if (clonePrefab != null)
        {
            GameObject clone = Instantiate(clonePrefab, playerTransform.position, Quaternion.identity);

            EnemyAI[] enemies = FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
            foreach (EnemyAI enemy in enemies)
            {
                enemy.target = clone.transform;
            }

            Destroy(clone, cloneDuration);

            /* Nota: Para que el aggro regrese al jugador al destruirse el clon, 
               deberás añadirle un script pequeñito al 'clonePrefab' que, en su OnDestroy(), 
               vuelva a hacer un FindObjectsByType y regrese el target al "Player". */
        }
    }
}
