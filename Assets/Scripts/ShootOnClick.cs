using UnityEngine;
using UnityEngine.InputSystem;

public class ShootOnClick : ShooterBase
{
    private void Update()
    {
        if (Mouse.current == null) return;

        // isPressed permite mantener el click; usa wasPressedThisFrame para un disparo por click
        if (Mouse.current.leftButton.isPressed)
        {
            Shoot(GetMouseDirection());
        }
    }
}