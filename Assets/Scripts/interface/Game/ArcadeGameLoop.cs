using System;
using System.Collections;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;

public class ArcadeGameLoop : MonoBehaviour
{
    public enum GamePhase { Fase1, Fase2, Climax, Jefe, GameOver }

    [Header("Estadísticas de la partida")]
    public int totalKills = 0;
    public int bossesDefeated = 0;
    public int maxCombo = 1;
    public float timeSurvived = 0f;
    public int difficultyLevel = 0; //sube con cada jefe derrotado; el Spawner puede usarlo

    [Header("Estado de Partida")]
    public GamePhase currentPhase = GamePhase.Fase1;
    public int currentScore = 0;
    public bool isGameActive = false;

    [Header("Sistema de Combos")]
    public int comboMultiplier = 1;
    private const int MAX_COMBO = 10;
    private const float COMBO_TIMEOUT = 3f;
    private float lastKillTime = 0f;

    [Header("Progresión de Oleadas")]
    public int killCount = 0;
    public int killsForFase2 = 15;
    public int killsForClimax = 35;
    public int killsForBoss = 50;

    // "Action<int>" significa que este evento enviará un número entero (el puntaje) a quien lo "escuche".
    public event Action<int> OnScoreUpdated;
    public event Action<int> OnComboUpdated;
    public event Action<GamePhase> OnPhaseChanged;
    public event Action OnBossSpawnRequested;

    public void StartGame()
    {
        StopAllCoroutines(); // evita duplicar la corrutina de puntos por tiempo
        isGameActive = true;
        currentPhase = GamePhase.Fase1;
        currentScore = 0;
        killCount = 0;
        totalKills = 0;
        bossesDefeated = 0;
        maxCombo = 1;
        timeSurvived = 0f;
        difficultyLevel = 0;
        ResetCombo();

        // Iniciamos la corrutina que da puntos por tiempo
        StartCoroutine(SurvivalScoreRoutine());

        // Avisamos a cualquier script que la fase cambió
        OnPhaseChanged?.Invoke(currentPhase);
        OnScoreUpdated?.Invoke(currentScore);
    }

    private void Update()
    {
        if (!isGameActive) return;
        timeSurvived += Time.deltaTime;

        //Si pasa mucho tiempo sin matar, el combo vuelve a x1
        if (comboMultiplier > 1 && Time.time - lastKillTime > COMBO_TIMEOUT)
            ResetCombo();
    }

    // Corrutina que otorga 10 puntos automáticamente por cada segundo de supervivencia
    private IEnumerator SurvivalScoreRoutine()
    {
        while (isGameActive)
        {
            yield return new WaitForSeconds(1f);
            AddScore(10);
        }
    }

    //Los enemigos deben llamar a esta función justo al morir.
    public void RegisterKill(bool isBoss = false)
    {
        if (!isGameActive) return;

        if (isBoss)
        {
            AddScore(5000); // 5000 puntos directos por derrotar al Jefe
            DefeatBoss();
        }
        else
        {
            AddScore(100 * comboMultiplier); // 100 puntos base multiplicados por el combo actual
            IncreaseCombo();
            killCount++; // Aumentamos el contador para saber cuándo cambiar de fase de oleadas
            totalKills++;
            CheckPhaseProgression();
        }
    }

    //Llamar desde el script de salud del Jugador cuando reciba un golpe.
    public void RegisterPlayerDamage()
    {
        if (!isGameActive) return;
        ResetCombo(); // perder el multiplicador al recibir daño
    }

    private void AddScore(int amount)
    {
        currentScore += amount;
        OnScoreUpdated?.Invoke(currentScore);
    }

    private void IncreaseCombo()
    {
        lastKillTime = Time.time; // Guardamos el momento exacto de la baja para reiniciar el temporizador
        if (comboMultiplier < MAX_COMBO)
        {
            comboMultiplier++;
            OnComboUpdated?.Invoke(comboMultiplier);
        }
        maxCombo = Mathf.Max(maxCombo, comboMultiplier);
    }

    private void ResetCombo()
    {
        comboMultiplier = 1;
        OnComboUpdated?.Invoke(comboMultiplier);
    }

    // Revisa si ya matamos suficientes enemigos para avanzar a la siguiente dificultad de oleadas
    private void CheckPhaseProgression()
    {
        if (currentPhase == GamePhase.Fase1 && killCount >= killsForFase2)
            AdvancePhase(GamePhase.Fase2);
        else if (currentPhase == GamePhase.Fase2 && killCount >= killsForClimax)
            AdvancePhase(GamePhase.Climax);
        else if (currentPhase == GamePhase.Climax && killCount >= killsForBoss)
            AdvancePhase(GamePhase.Jefe);
    }

    private void AdvancePhase(GamePhase newPhase)
    {
        currentPhase = newPhase;
        OnPhaseChanged?.Invoke(currentPhase); // Avisa al Spawner que debe cambiar los tipos de enemigos a generar

        if (currentPhase == GamePhase.Jefe)
            OnBossSpawnRequested?.Invoke(); // Dispara el evento específico para invocar al Jefe de la arena
    }

    private void DefeatBoss()
    {
        bossesDefeated++; 
        difficultyLevel++;
        // Al derrotar al jefe, reiniciamos el conteo y volvemos a la Fase 1.
        killCount = 0;
        AdvancePhase(GamePhase.Fase1);
    }

    // Se llama cuando la vida del Campeón llega a 0
    public void TriggerGameOver()
    {
        if (!isGameActive) return; //evita llamarlo dos veces
        isGameActive = false;
        currentPhase = GamePhase.GameOver;
        StopAllCoroutines(); // Detiene la suma de puntos por supervivencia
        OnPhaseChanged?.Invoke(currentPhase);

        // Aquí deberíamos guardar el currentScore como High Score en PlayerPrefs o enviarlo al gestor de guardado.
    }

    public GameResult BuildResult() => new GameResult
    {
        score = currentScore,
        kills = totalKills,
        bossesDefeated = bossesDefeated,
        maxCombo = maxCombo,
        timeSurvived = timeSurvived
    };
}

public struct GameResult
{
    public int score;
    public int kills;
    public int bossesDefeated;
    public int maxCombo;
    public float timeSurvived;
}
