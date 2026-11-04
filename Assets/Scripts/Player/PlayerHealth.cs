using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private ArcadeGameLoop gameLoop;
    [SerializeField] private float invulnerabilitySeconds = 0.5f; //evita morir en un solo frame

    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsInvulnerable { get; set; } //para el dash

    public event Action<int, int> OnHealthChanged; //vida actual, vida máxima
    public event Action OnDied;

    private float defensePercent;
    private float invulnerableUntil;

    public void Initialize(CharacterData data)
    {
        MaxHealth = data.maxHealth;
        CurrentHealth = MaxHealth;
        defensePercent = data.defensePercent;
        IsDead = false;
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public void TakeDamage(int rawDamage)
    {
        if (IsDead || IsInvulnerable || Time.time < invulnerableUntil) return;

        int damage = Mathf.Max(1, Mathf.RoundToInt(rawDamage * (1f - defensePercent / 100f)));
        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
        invulnerableUntil = Time.time + invulnerabilitySeconds;

        gameLoop.RegisterPlayerDamage(); //el combo vuelve a x1
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth == 0)
        {
            IsDead = true;
            OnDied?.Invoke();
        }
    }

    public void Heal(int amount) //orbes de curación de los cofres
    {
        if (IsDead) return;
        CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    // Temporal: daño por contacto hasta que los enemigos tengan su propio script
    //private void OnCollisionStay2D(Collision2D other)
    //{
      //  if (other.collider.CompareTag("Enemy")) TakeDamage(10);
    //}

#if UNITY_EDITOR
    private void Update()
    {
        //atajo de prueba: K = recibir 25 de daño
        if (Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame)
            TakeDamage(25);
    }
#endif
}