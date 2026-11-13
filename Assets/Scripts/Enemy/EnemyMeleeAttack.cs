using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
public class EnemyMeleeAttack : MonoBehaviour
{
    [Header("Ataque")]
    [SerializeField] private float attackRange = 1.4f;                 // distancia a la que empieza a atacar
    [SerializeField] private float hitRange = 1.7f;                    // alcance real del golpe (algo mayor que attackRange)
    [SerializeField, Range(10f, 360f)] private float attackArc = 120f;
    [SerializeField] private float windupTime = 0.4f;                  // aviso antes del golpe: el jugador puede esquivar
    [SerializeField] private float recoveryTime = 0.3f;                // pausa después del golpe
    [SerializeField] private float attackCooldown = 1.5f;              // tiempo entre ataques (desde que empieza uno)

    [Header("Aviso visual (opcional)")]
    [SerializeField] private SpriteRenderer telegraphRenderer;         // si está vacío, usa el primer SpriteRenderer hijo
    [SerializeField] private Color telegraphColor = new Color(1f, 0.35f, 0.35f);

    // Ganchos para conectar la animación más adelante
    public event Action OnAttackStarted;
    public event Action OnAttackHit;

    private EnemyAI ai;
    private float nextAttackTime;
    private Color originalColor = Color.white;

    private void Awake()
    {
        ai = GetComponent<EnemyAI>();
        if (telegraphRenderer == null) telegraphRenderer = GetComponentInChildren<SpriteRenderer>();
        if (telegraphRenderer != null) originalColor = telegraphRenderer.color;
    }

    private void Update()
    {
        if (ai.currentState != EnemyAI.EnemyState.Chasing || ai.target == null) return;
        if (!ShooterBase.AttacksEnabled) return;     // no ataca en intro, pausa ni Game Over
        if (Time.time < nextAttackTime) return;

        float distance = Vector2.Distance(transform.position, ai.target.position);
        if (distance <= attackRange) StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        nextAttackTime = Time.time + attackCooldown;
        ai.BeginAttack();
        OnAttackStarted?.Invoke();

        // La dirección se fija al inicio: si el jugador se mueve durante el aviso, puede esquivar
        Vector2 aimDirection = ((Vector2)ai.target.position - (Vector2)transform.position).normalized;
        SetTelegraph(true);

        yield return new WaitForSeconds(windupTime);
        SetTelegraph(false);

        // Si lo aturdieron durante el aviso, el ataque se cancela
        if (ai.currentState != EnemyAI.EnemyState.Attacking) yield break;

        TryHitPlayer(aimDirection);

        yield return new WaitForSeconds(recoveryTime);
        ai.EndAttack();
    }

    private void TryHitPlayer(Vector2 aimDirection)
    {
        if (ai.target == null) return;

        Vector2 toPlayer = (Vector2)ai.target.position - (Vector2)transform.position;
        if (toPlayer.magnitude > hitRange) return;                                // el jugador se alejó
        if (Vector2.Angle(aimDirection, toPlayer) > attackArc * 0.5f) return;     // salió del arco

        PlayerHealth health = ai.target.GetComponent<PlayerHealth>();
        if (health == null) return;

        health.TakeDamage(ai.damage);   // respeta la invulnerabilidad del dash
        OnAttackHit?.Invoke();
    }

    private void SetTelegraph(bool on)
    {
        if (telegraphRenderer != null) telegraphRenderer.color = on ? telegraphColor : originalColor;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitRange);
    }
}