using UnityEngine;

public class ShootAimMouse : ShooterBase
{
    [SerializeField] private Transform weaponPivot; // opcional: arma/torreta que rota

    private void Update()
    {
        Vector2 dir = GetMouseDirection();

        if (weaponPivot != null)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            weaponPivot.rotation = Quaternion.Euler(0, 0, angle);
        }

        // Dispara continuamente respetando el fireRate
        Shoot(dir);
    }
}