using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    public enum GameState { Countdown, Playing, Paused, GameOver }

    [Header("Referencias")]
    [SerializeField] private ArcadeGameLoop gameLoop;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameFlowUI ui;
    [SerializeField] private CharacterData fallbackCharacter; //para probar la escena Game (ej: Thor.asset)

    [Header("Configuración")]
    [SerializeField] private int countdownSeconds = 3;
    [SerializeField] private float deathSlowMoSeconds = 1.2f;
    [SerializeField] private string menuScene = "Menu";
    [SerializeField] private string characterSelectScene = "CharacterSelect";
    [SerializeField] private PlayerVisuals playerVisuals;
    [SerializeField] private PlayerAbilities playerAbilities;

    public GameState State { get; private set; } = GameState.Countdown;

    private CharacterData character;
    private bool isLoading;

    private void Awake()
    {
        Time.timeScale = 1f; //por si venimos de una pausa
        SetPlayerControl(false);
    }

    private void OnEnable() => playerHealth.OnDied += HandlePlayerDied;
    private void OnDisable() => playerHealth.OnDied -= HandlePlayerDied;

    private IEnumerator Start()
    {
        character = GameSession.SelectedCharacter != null ? GameSession.SelectedCharacter : fallbackCharacter;
        GameSession.SelectedCharacter = character;
        ApplyCharacterStats(character);

        yield return ui.PlayIntro(character, countdownSeconds);

        State = GameState.Playing;
        SetPlayerControl(true);
        gameLoop.StartGame();
    }

    private void ApplyCharacterStats(CharacterData data)
    {
        playerVisuals.Apply(data);
        playerAbilities.Setup(data);
        playerHealth.Initialize(data);

        playerMovement.SpeedMultiplier = data.moveSpeedMultiplier;

        PlayerDash dash = playerMovement.GetComponent<PlayerDash>();
        if (dash != null) dash.EvasionMultiplier = data.evasionMultiplier;

        foreach (ShooterBase shooter in playerMovement.GetComponentsInChildren<ShooterBase>(true))
        {
            shooter.AttackSpeedMultiplier = data.attackSpeedMultiplier;
            shooter.AttackDamage = data.attackPoints;
        }
        //attackPoints (daño de Bullet) y evasionMultiplier (dash) se conectan cuando existan esos sistemas

        // El ataque cuerpo a cuerpo recibe daño y velocidad del personaje
        MeleeAttack melee = playerMovement.GetComponentInChildren<MeleeAttack>(true);
        if (melee != null)
        {
            melee.AttackSpeedMultiplier = data.attackSpeedMultiplier;
            melee.AttackDamage = data.attackPoints;
        }

        // Cada personaje usa melee o disparo a distancia según su estilo
        bool usesRanged = data.attackStyle == AttackStyle.Ranged;
        if (melee != null) melee.enabled = !usesRanged;

        ShootOnClick rangedShooter = playerMovement.GetComponentInChildren<ShootOnClick>(true);
        if (rangedShooter != null) rangedShooter.enabled = usesRanged;
    }

    private void SetPlayerControl(bool enabled)
    {
        playerMovement.CanMove = enabled;
        ShooterBase.AttacksEnabled = enabled;
    }

    private void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame) return;

        if (State == GameState.Playing) Pause();
        else if (State == GameState.Paused) Resume();
    }

    public void Pause()
    {
        if (State != GameState.Playing) return;
        State = GameState.Paused;
        Time.timeScale = 0f;
        SetPlayerControl(false);
        ui.ShowPause(true);
    }

    public void Resume()
    {
        if (State != GameState.Paused) return;
        State = GameState.Playing;
        Time.timeScale = 1f;
        SetPlayerControl(true);
        ui.ShowPause(false);
    }

    private void HandlePlayerDied()
    {
        if (State == GameState.GameOver) return;
        State = GameState.GameOver;

        gameLoop.TriggerGameOver();
        SetPlayerControl(false);
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        //guardamos primero, por si el jugador cierra el juego durante la animación
        GameResult result = gameLoop.BuildResult();
        int previousBest = HighScoreService.BestScore;
        HighScoreService.Submit(new ScoreEntry
        {
            score = result.score,
            characterName = character.characterName,
            kills = result.kills,
            timeSurvived = result.timeSurvived,
            date = DateTime.Now.ToString("dd/MM/yyyy")
        });
        bool isNewRecord = result.score > previousBest;

        Time.timeScale = 0.25f; //cámara lenta de muerte
        yield return new WaitForSecondsRealtime(deathSlowMoSeconds);
        Time.timeScale = 0f;    //congela la arena detrás del menú

        ui.ShowGameOver(result, previousBest, isNewRecord);
    }

    //botones
    public void Retry() => LoadScene(SceneManager.GetActiveScene().name);
    public void ChangeCharacter() => LoadScene(characterSelectScene);
    public void BackToMenu() => LoadScene(menuScene);

    private void LoadScene(string sceneName)
    {
        if (isLoading) return;
        isLoading = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}