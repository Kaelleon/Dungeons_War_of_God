using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;   // opcional
    [SerializeField] private Transform godRoot;   // hijo vacío del Player donde se instancia el godPrefab

    private GameObject currentGodInstance;

    public void Apply(CharacterData data)
    {
        bool useAnimator = animator != null && data.animatorController != null;

        // Si el Animator está activo, pisa el sprite en cada frame, por eso se apaga cuando no hay controller
        if (animator != null) animator.enabled = useAnimator;

        if (useAnimator)
            animator.runtimeAnimatorController = data.animatorController;
        else if (data.gameSprite != null && spriteRenderer != null)
            spriteRenderer.sprite = data.gameSprite;

        if (currentGodInstance != null) Destroy(currentGodInstance);

        if (data.godPrefab != null)
            currentGodInstance = Instantiate(data.godPrefab, godRoot != null ? godRoot : transform);
    }
}