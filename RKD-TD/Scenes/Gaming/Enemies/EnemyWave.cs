using System;

namespace RKD_TD.Scenes.Gaming.Enemies;

internal sealed class EnemyWave
{
    public float SpawnInterval { get; }
    public string[] EnemiesToSpawn { get; }

    public int Reward { get; }

    public EnemyWave(
        string[] enemiesToSpawn,
        int reward,
        int waveNumber,
        float basicSpawnInterval,
        float spawnIntervalDecrease)
    {
        SpawnInterval = MathF.Max(0.3f, basicSpawnInterval - waveNumber * spawnIntervalDecrease);
        EnemiesToSpawn = enemiesToSpawn;
        Reward = reward;
    }
}