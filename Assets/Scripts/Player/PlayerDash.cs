using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerMovement), typeof(PlayerHealth))]
public class PlayerDash : MonoBehaviour
{
    [Header("Configuración Base")]
    public float dashBaseSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashBaseCooldown = 2f;

    public float EvasionMultiplier { get; set; } = 1f; // Se conectará desde el GameFlowManager

    private Rigidbody2D rb;
    private PlayerMovement movement;
    private PlayerHealth health;

    private bool isDashing = false;
    private float lastDashTime = -10f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement>();
        health = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (!movement.CanMove) return;

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            float currentCooldown = dashBaseCooldown / Mathf.Max(0.1f, EvasionMultiplier);

            if (!isDashing && Time.time >= lastDashTime + currentCooldown)
                StartCoroutine(DashRoutine());
        }
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        lastDashTime = Time.time;

        movement.CanMove = false;
        health.IsInvulnerable = true;

        Vector2 dashDir = rb.linearVelocity.normalized;
        if (dashDir == Vector2.zero) dashDir = Vector2.right;

        rb.linearVelocity = dashDir * (dashBaseSpeed * EvasionMultiplier);

        yield return new WaitForSeconds(dashDuration);

        health.IsInvulnerable = false;  
        movement.CanMove = true;
        isDashing = false;
    }
}
