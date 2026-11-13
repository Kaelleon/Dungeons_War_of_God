using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Ataque cuerpo a cuerpo del jugador: apunta con el mouse y, mientras mantiene el click,
// ataca en esa dirección cada "attackCooldown" segundos.
public class MeleeAttack : MonoBehaviour
{
    [Header("Ataque")]
    [SerializeField] private float attackCooldown = 0.5f;              // segundos entre ataques
    [SerializeField] private float attackRange = 1.6f;                 // alcance desde el centro del jugador
    [SerializeField, Range(10f, 360f)] private float attackArc = 120f; // ángulo del golpe (grados)
    [SerializeField] private float hitDelay = 0f;                      // retraso entre iniciar el ataque y aplicar el daño (para la animación)

    [Header("Feedback (opcional)")]
    [SerializeField] private Transform slashVisual;                    // objeto que se muestra un instante al atacar
    [SerializeField] private float slashDuration = 0.12f;

    public float AttackSpeedMultiplier { get; set; } = 1f;
    public int AttackDamage { get; set; } = 10;

    // Ganchos para conectar animación y sonido más adelante
    public event Action<Vector2> OnAttackStarted;   // dirección del golpe
    public event Action<int> OnAttackHit;           // cantidad de enemigos golpeados

    private PlayerMovement movement;
    private float nextAttackTime;
    private Vector2 aimDirection = Vector2.right;
    private readonly HashSet<GameObject> hitThisSwing = new HashSet<GameObject>();

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        if (slashVisual != null) slashVisual.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Mouse.current == null || Camera.main == null) return;

        aimDirection = GetMouseDirection();

        if (!ShooterBase.AttacksEnabled) return;              // intro, pausa y Game Over
        if (movement != null && !movement.CanMove) return;    // también bloquea el ataque durante el dash

        // isPressed: mantener el click ataca de forma continua (usa wasPressedThisFrame para un golpe por click)
        if (Mouse.current.leftButton.isPressed && Time.time >= nextAttackTime)
            StartAttack(aimDirection);
    }

    private void StartAttack(Vector2 direction)
    {
        nextAttackTime = Time.time + attackCooldown / Mathf.Max(0.1f, AttackSpeedMultiplier);
        OnAttackStarted?.Invoke(direction);

        if (slashVisual != null) StartCoroutine(ShowSlash(direction));

        if (hitDelay <= 0f) ApplyHit(direction);
        else StartCoroutine(HitAfterDelay(direction));
    }

    private IEnumerator HitAfterDelay(Vector2 direction)
    {
        yield return new WaitForSeconds(hitDelay);
        if (!ShooterBase.AttacksEnabled) yield break;
        ApplyHit(direction);
    }

    private void ApplyHit(Vector2 direction)
    {
        Vector2 origin = transform.position;
        hitThisSwing.Clear();
        int enemiesHit = 0;

        foreach (Collider2D hit in Physics2D.OverlapCircleAll(origin, attackRange))
        {
            if (!hit.CompareTag("Enemy")) continue;

            // Un enemigo cuenta una sola vez por golpe, aunque tenga varios colliders
            GameObject enemy = hit.attachedRigidbody != null ? hit.attachedRigidbody.gameObject : hit.gameObject;
            if (!hitThisSwing.Add(enemy)) continue;

            // Se usa el punto más cercano del collider, así un jefe grande también entra en el arco
            Vector2 toEnemy = hit.ClosestPoint(origin) - origin;
            if (toEnemy.sqrMagnitude > 0.0001f && Vector2.Angle(direction, toEnemy) > attackArc * 0.5f) continue;

            Vector2 knockbackDir = (Vector2)enemy.transform.position - origin;
            knockbackDir = knockbackDir.sqrMagnitude > 0.0001f ? knockbackDir.normalized : direction;

            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(AttackDamage, knockbackDir);
                enemiesHit++;
                continue;
            }

            BossHealth boss = enemy.GetComponent<BossHealth>();
            if (boss != null)
            {
                boss.TakeDamage(AttackDamage, knockbackDir);
                enemiesHit++;
            }
        }

        OnAttackHit?.Invoke(enemiesHit);
    }

    private IEnumerator ShowSlash(Vector2 direction)
    {
        slashVisual.right = direction;   // apunta hacia donde se ataca
        slashVisual.gameObject.SetActive(true);
        yield return new WaitForSeconds(slashDuration);
        slashVisual.gameObject.SetActive(false);
    }

    private Vector2 GetMouseDirection()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 dir = (Vector2)mouseWorld - (Vector2)transform.position;
        return dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector2.right;
    }

    // Muestra el alcance y el arco en el editor (sigue al mouse en Play)
    private void OnDrawGizmosSelected()
    {
        Vector2 dir = Application.isPlaying ? aimDirection : Vector2.right;
        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Vector3 p = transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(p, attackRange);
        Gizmos.DrawLine(p, p + Quaternion.Euler(0, 0, baseAngle + attackArc * 0.5f) * Vector3.right * attackRange);
        Gizmos.DrawLine(p, p + Quaternion.Euler(0, 0, baseAngle - attackArc * 0.5f) * Vector3.right * attackRange);
    }
}