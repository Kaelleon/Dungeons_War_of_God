using UnityEngine;

public class AbilityBarUI : MonoBehaviour
{
    [SerializeField] private PlayerAbilities playerAbilities;
    [SerializeField] private AbilitySlotHUD[] slots;   // en orden: tecla 1, 2, 3

    private static readonly string[] Keys = { "1", "2", "3" };

    private void OnEnable()
    {
        playerAbilities.OnAbilitiesChanged += Rebuild;
        playerAbilities.OnAbilityUsed += HandleAbilityUsed;
        Rebuild();
    }

    private void OnDisable()
    {
        playerAbilities.OnAbilitiesChanged -= Rebuild;
        playerAbilities.OnAbilityUsed -= HandleAbilityUsed;
    }

    private void Rebuild()
    {
        for (int i = 0; i < slots.Length; i++)
            slots[i].Bind(playerAbilities.GetAbility(i), i < Keys.Length ? Keys[i] : "");
    }

    private void HandleAbilityUsed(int index)
    {
        if (index < slots.Length) slots[index].PlayUsed();
    }

    private void Update()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            AbilityLogic ability = playerAbilities.GetAbility(i);
            if (ability == null) continue;
            slots[i].Tick(playerAbilities.GetCooldownRemaining(i), ability.cooldown);
        }
    }
}