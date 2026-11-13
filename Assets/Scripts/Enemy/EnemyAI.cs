using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Spawning, Chasing, Stunned, Attacking }

    [Header("Estado Actual")]
    public EnemyState currentState = EnemyState.Spawning;

    [Header("Estadísticas")]
    public float moveSpeed = 3f;
    public int damage = 10;          // daño de cada ataque (lo usa EnemyMeleeAttack)
    public float spawnDelay = 0.5f;

    [HideInInspector] public Transform target;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) target = player.transform;

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        currentState = EnemyState.Spawning;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(spawnDelay);

        currentState = EnemyState.Chasing;
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        if (currentState == EnemyState.Chasing) MoveTowardsTarget();
        else if (currentState == EnemyState.Attacking) rb.linearVelocity = Vector2.zero; // se planta mientras ataca
    }

    protected virtual void MoveTowardsTarget()
    {
        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }

    // Lo llama EnemyMeleeAttack
    public void BeginAttack()
    {
        currentState = EnemyState.Attacking;
        rb.linearVelocity = Vector2.zero;
    }

    public void EndAttack()
    {
        if (currentState == EnemyState.Attacking) currentState = EnemyState.Chasing;
    }

    public void ApplyStun(float stunDuration, Vector2 knockbackForce)
    {
        if (currentState == EnemyState.Spawning) return;
        StartCoroutine(StunRoutine(stunDuration, knockbackForce));
    }

    private IEnumerator StunRoutine(float duration, Vector2 knockback)
    {
        currentState = EnemyState.Stunned;
        rb.linearVelocity = knockback;

        yield return new WaitForSeconds(duration);

        if (currentState == EnemyState.Stunned)
            currentState = EnemyState.Chasing;
    }
}