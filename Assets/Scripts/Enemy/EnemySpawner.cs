using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Referencias")]
    public ArcadeGameLoop gameLoop;
    public Transform[] spawnPoints; 

    [Header("Prefabs de Enemigos")]
    public GameObject basicPrefab;
    public GameObject hunterPrefab;
    public GameObject shooterPrefab;
    public GameObject tankPrefab;
    public GameObject bossPrefab;

    private bool isSpawning = false;
    private ArcadeGameLoop.GamePhase currentPhase;
    private Coroutine spawnCoroutine;

    private void OnEnable()
    {
        if (gameLoop != null)
        {
            gameLoop.OnPhaseChanged += HandlePhaseChanged;
            gameLoop.OnBossSpawnRequested += SpawnBoss;
        }
    }

    private void OnDisable()
    {
        if (gameLoop != null)
        {
            gameLoop.OnPhaseChanged -= HandlePhaseChanged;
            gameLoop.OnBossSpawnRequested -= SpawnBoss;
        }
    }

    private void HandlePhaseChanged(ArcadeGameLoop.GamePhase newPhase)
    {
        currentPhase = newPhase;

        if (currentPhase == ArcadeGameLoop.GamePhase.Fase1 ||
            currentPhase == ArcadeGameLoop.GamePhase.Fase2 ||
            currentPhase == ArcadeGameLoop.GamePhase.Climax)
        {
            if (!isSpawning)
            {
                isSpawning = true;
                spawnCoroutine = StartCoroutine(SpawnRoutine());
            }
        }
        else
        {
            isSpawning = false;
            if (spawnCoroutine != null)
                StopCoroutine(spawnCoroutine);
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            SpawnRandomEnemy();

            float waitTime = 2f;
            if (currentPhase == ArcadeGameLoop.GamePhase.Fase2) waitTime = 1.5f;
            else if (currentPhase == ArcadeGameLoop.GamePhase.Climax) waitTime = 1f;

            yield return new WaitForSeconds(waitTime);
        }
    }

    private void SpawnRandomEnemy()
    {
        if (spawnPoints.Length == 0) return;

        Transform randomSpawnPoint = spawnPoints[Random.Range(0,spawnPoints.Length)];
        GameObject enemyToSpawn = basicPrefab;

        float rand = Random.value;

        if (currentPhase == ArcadeGameLoop.GamePhase.Fase1)
            enemyToSpawn = (rand > 0.5f) ? basicPrefab : hunterPrefab;
        else if (currentPhase == ArcadeGameLoop.GamePhase.Fase2)
        {
            if (rand < 0.4f) enemyToSpawn = basicPrefab;
            else if (rand < 0.8f) enemyToSpawn = hunterPrefab;
            else enemyToSpawn = shooterPrefab;
        }
        else if (currentPhase == ArcadeGameLoop.GamePhase.Climax)
        {
            if (rand < 0.3f) enemyToSpawn = basicPrefab;
            else if (rand < 0.6f) enemyToSpawn = hunterPrefab;
            else if (rand < 0.85f) enemyToSpawn = shooterPrefab;
            else enemyToSpawn = tankPrefab;
        }

        if (enemyToSpawn != null)
            Instantiate(enemyToSpawn, randomSpawnPoint.position, Quaternion.identity);
    }

    private void SpawnBoss()
    {
        if (spawnPoints.Length == 0 || bossPrefab == null) return;

        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(bossPrefab, randomSpawnPoint.position, Quaternion.identity);
    }
}
