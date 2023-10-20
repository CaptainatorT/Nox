
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemyWaveData
{
    public BaseEnemy enemyPrefab;
    public int initialCount;
    public int startWave;  // Start wave for this enemy type to appear
    public int endWave;    // End wave for this enemy type. 0 means endless
    public int incrementPerWave;

    public int initialChunkSize;
    public int incrementChunkSize;
    public int chunkSizeIncreaseInterval;
}