using UnityEngine;

[System.Serializable]
public class EnemyUnit
{
    [Tooltip("Time into the wave before this enemy starts spawning.")]
    public float spawnStartTime = 0f;
    public GameObject enemyPrefab;
    public int spawnCount;
    public float spawnInterval;
    public bool isPriority;
    public float spawnDistanceFromPlayer;  // Add this line for the spawn distance
    [HideInInspector]
    public float nextSpawnTime;
    [HideInInspector]
    public int spawnedCount = 0;
}
