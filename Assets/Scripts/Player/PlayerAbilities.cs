using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerAbilities : MonoBehaviour
{
    private AbilityLogic[] abilities = new AbilityLogic[3];
    private float[] nextAvailableTimes = new float[3];
    private PlayerMovement movement;

    private void Start()
    {
        movement = GetComponent<PlayerMovement>();

        if (GameSession.SelectedCharacter != null)
        {
            for (int i = 0; i < 3; i++)
            {
                if (i < GameSession.SelectedCharacter.abilities.Length)
                    abilities[i] = GameSession.SelectedCharacter.abilities[i] as AbilityLogic;
            }
        }
    }

    private void Update()
    {
        if (movement != null && !movement.CanMove) return;
        if (Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame) TryActivateAbility(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) TryActivateAbility(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) TryActivateAbility(2);
    }

    private void TryActivateAbility(int index)
    {
        if (index >= 0 && index < abilities.Length && abilities[index] != null)
        {
            if (Time.time >= nextAvailableTimes[index])
            {
                abilities[index].Activate(transform);
                nextAvailableTimes[index] = Time.time + abilities[index].cooldown;

                Debug.Log($"¡Poder Divino activado: {abilities[index].abilityName}!");
            }
        }
    }
}
