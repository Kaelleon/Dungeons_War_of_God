using UnityEngine;

public class AggroDecoy : MonoBehaviour
{
    private void OnDestroy()
    {
        // Evita errores al cerrar o recargar la escena
        if (!gameObject.scene.isLoaded) return;

        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player == null) return;

        foreach (EnemyAI enemy in FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
        {
            // Solo los que seguían a ESTE clon (si hay dos clones, no se pisan)
            if (enemy.target == transform) enemy.target = player.transform;
        }
    }
}