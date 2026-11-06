using UnityEngine;

public class RewardChest : MonoBehaviour
{
    [Header("Recompensas")]
    public int healAmount = 50; 

    private bool isOpened = false;

    // Este método será llamado por el jugador al presionar 'E'
    public void OpenChest(GameObject player)
    {
        if (isOpened) return;
        isOpened = true;

        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.Heal(healAmount);
            Debug.Log("¡Cofre abierto! +" + healAmount + " HP");
        }

        // A futuro: Aquí abriremos la UI para elegir los objetos de Buffeo Leve de las 3 casillas

        Destroy(gameObject);
    }
}
