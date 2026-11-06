using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerAbilities : MonoBehaviour
{
    private readonly AbilityLogic[] abilities = new AbilityLogic[3];
    private readonly float[] nextAvailableTimes = new float[3];
    private PlayerMovement movement;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }

    // Lo llama GameFlowManager al iniciar la escena, con el personaje elegido
    public void Setup(CharacterData character)
    {
        for (int i = 0; i < abilities.Length; i++)
        {
            abilities[i] = null;
            nextAvailableTimes[i] = 0f;

            if (character == null || i >= character.abilities.Length) continue;

            AbilityData data = character.abilities[i];
            abilities[i] = data as AbilityLogic;

            // Avisa si el slot tiene una habilidad que aún no tiene lógica de combate
            if (data != null && abilities[i] == null)
                Debug.LogWarning("'" + data.abilityName + "' no tiene lógica de combate todavía.");
        }
    }

    private void Update()
    {
        // CanMove es false en la intro, la pausa y el Game Over
        if (movement != null && !movement.CanMove) return;
        if (Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame) TryActivateAbility(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) TryActivateAbility(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) TryActivateAbility(2);
    }

    private void TryActivateAbility(int index)
    {
        if (index < 0 || index >= abilities.Length || abilities[index] == null) return;
        if (Time.time < nextAvailableTimes[index]) return;

        abilities[index].Activate(transform);
        nextAvailableTimes[index] = Time.time + abilities[index].cooldown;
    }
}