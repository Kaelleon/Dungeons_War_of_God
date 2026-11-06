using UnityEngine;

// Heredamos de AbilityData para que encaje perfectamente en tu CharacterData actual
public abstract class AbilityLogic : AbilityData
{
    [Header("Lógica de Combate")]
    public float cooldown = 5f;

    // Método abstracto que cada dios programará a su manera
    public abstract void Activate(Transform playerTransform);
}