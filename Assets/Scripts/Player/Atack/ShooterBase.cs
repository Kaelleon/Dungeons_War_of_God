using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ShooterBase : MonoBehaviour
{
    [Header("Disparo")]
    [SerializeField] protected Bullet bulletPrefab;
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected float fireRate = 0.3f; // segundos entre disparos

    private float nextFireTime;

    protected bool CanShoot => Time.time >= nextFireTime;

    protected void Shoot(Vector2 direction)
    {
        if (!CanShoot) return;
        nextFireTime = Time.time + fireRate;

        Bullet bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.Launch(direction);
    }

    protected Vector2 GetMouseDirection()
    {
        if (Mouse.current == null) return Vector2.right; // por si no hay mouse conectado

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(screenPos);
        mouseWorld.z = 0f;
        return (mouseWorld - firePoint.position).normalized;
    }
}