using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class WaveConfig
{
    public int waveNumber;
    public float waveDuration;
    public List<EnemyUnit> enemyUnits;
    public Vector2 spawnDirection; // Stores the angle or direction of spawning.
}
