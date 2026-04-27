namespace RKD_TD.Scenes.Gaming.Enemies;

internal sealed class EnemyWave
{
    public float SpawnInterval { get; }
    public string[] EnemiesToSpawn { get; }

    public int Reward { get; }

    public EnemyWave(
        float spawnTime,
        string[] enemiesToSpawn,
        int reward)
    {
        SpawnInterval = spawnTime / enemiesToSpawn.Length;
        EnemiesToSpawn = enemiesToSpawn;
        Reward = reward;
    }
}