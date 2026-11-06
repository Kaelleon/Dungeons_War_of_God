using UnityEngine;

public abstract class AbilityLogic : AbilityData
{
    [Header("Lógica de Combate")]
    public float cooldown = 5f;

    public abstract void Activate(Transform playerTransform);
}
