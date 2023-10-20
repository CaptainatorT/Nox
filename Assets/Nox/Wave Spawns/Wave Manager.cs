using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    public List<WaveConfig> waveConfigs;
    private int currentWaveIndex = 0;
    public bool waveActive = true;

    public UnityEvent onWaveStarted;
    public UnityEvent onWaveEnded;
    public float waveDelay = 5.0f;  // Delay between waves
    public int CurrentWaveIndex
    {
        get { return currentWaveIndex; }
    }

    public UIManager uiManager; // Reference to the UIManager

    private float waveTimer = 0f;
    public float minSpawnRadius = 10f; // minimum distance from the player where enemies can spawn
    public float maxSpawnRadius = 20f; // maximum distance for enemy spawning
    private Transform playerTransform;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }

        foreach (var waveConfig in waveConfigs)
        {
            foreach (var unit in waveConfig.enemyUnits)
            {
                unit.nextSpawnTime = 0;
            }
        }
    }
    void Update()
    {
        if (!waveActive) return; // Exit early if the wave is not active

        waveTimer += Time.deltaTime;

        // Spawn logic
        foreach (EnemyUnit unit in waveConfigs[currentWaveIndex].enemyUnits)
        {
            if (waveTimer >= unit.spawnStartTime && Time.time > unit.nextSpawnTime && (unit.spawnCount == 0 || unit.spawnedCount < unit.spawnCount))
            {
                Vector3 spawnPosition = GetRandomSpawnPointForEnemy(unit);
                Instantiate(unit.enemyPrefab, spawnPosition, Quaternion.identity);
                unit.spawnedCount++;

                // Reset the next spawn time using this unit's specific spawn interval
                unit.nextSpawnTime = Time.time + unit.spawnInterval;
            }
        }

        // Update the waveProgressSlider directly with the remaining time
        float remainingTime = waveConfigs[currentWaveIndex].waveDuration - waveTimer;
        uiManager.waveProgressSlider.value = 1 - (remainingTime / waveConfigs[currentWaveIndex].waveDuration);

        if (waveTimer >= waveConfigs[currentWaveIndex].waveDuration)
        {
            EndWave();
        }
    }

    public WaveConfig CurrentWaveConfig
    {
        get
        {
            if (currentWaveIndex >= 0 && currentWaveIndex < waveConfigs.Count)
                return waveConfigs[currentWaveIndex];
            else
                return null;  // or some default value
        }
    }
    public float CurrentWaveTime
    {
        get { return waveTimer; }
    }

    private Vector3 GetRandomSpawnPoint()
    {
        float randomAngle = UnityEngine.Random.Range(0, 360);
        Vector2 direction = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)).normalized;
        float spawnDistance = UnityEngine.Random.Range(minSpawnRadius, maxSpawnRadius);

        return playerTransform.position + new Vector3(direction.x, direction.y, 0) * spawnDistance;
    }
    private Vector3 GetRandomSpawnPointForEnemy(EnemyUnit unit)
    {
        // Determine a random angle between min and max radii for direction.
        float randomAngle = UnityEngine.Random.Range(minSpawnRadius, maxSpawnRadius);
        Vector2 direction = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad)).normalized;

        // Use the enemy's specific spawn distance to determine how far from the player it should spawn.
        float spawnDistance = unit.spawnDistanceFromPlayer;

        return playerTransform.position + new Vector3(direction.x, direction.y, 0) * spawnDistance;
    }

    void EndWave()
    {
        waveActive = false;  // Set the wave to inactive when it ends

        onWaveEnded?.Invoke();

        // Update the UIManager to notify wave completion
        uiManager.OnWaveCompleted();

        // Move to the next wave if there is one
        if (currentWaveIndex < waveConfigs.Count - 1)
        {
            // Inform the UIManager about the delay before the next wave
            uiManager.StartCountdown(waveDelay);

            StartCoroutine(WaitForNextWave());
        }
        else
        {
            // End of all waves logic
        }
    }

    private IEnumerator WaitForNextWave()
    {
        yield return new WaitForSeconds(waveDelay);

        currentWaveIndex++;
        Debug.Log("Advancing to Wave: " + currentWaveIndex); // Add this
        waveTimer = 0;
        waveActive = true;  // Activate the wave when the delay ends
    }

}

