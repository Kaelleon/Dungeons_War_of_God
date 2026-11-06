using UnityEngine;

public class RewardChest : MonoBehaviour
{
    [Header("Recompensas")]
    public int healAmount = 50; // Orbe de curación instantáneo[cite: 177]

    private bool isOpened = false;

    // Este método será llamado por el jugador al presionar 'E'
    public void OpenChest(GameObject player)
    {
        if (isOpened) return;
        isOpened = true;

        // 1. Curar al jugador usando el script que ya tienes[cite: 247]
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.Heal(healAmount);
            Debug.Log("¡Cofre abierto! +" + healAmount + " HP");
        }

        // A futuro: Aquí abriremos la UI para elegir los objetos de Buffeo Leve de las 3 casillas[cite: 177]

        Destroy(gameObject);
    }
}