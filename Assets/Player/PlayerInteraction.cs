using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Detección")]
    public float interactionRadius = 1.5f;
    public LayerMask interactableLayer; // Capa exclusiva para cofres y objetos

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        // Busca cualquier objeto interactuable cerca del jugador
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactionRadius, interactableLayer);

        foreach (Collider2D hit in hits)
        {
            RewardChest chest = hit.GetComponent<RewardChest>();
            if (chest != null)
            {
                chest.OpenChest(gameObject);
                break; // Solo abre un cofre por pulsación
            }
        }
    }
}
