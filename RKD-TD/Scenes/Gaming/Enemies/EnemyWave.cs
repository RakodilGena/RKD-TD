namespace RKD_TD.Scenes.Gaming.Enemies;

internal sealed class EnemyWave
{
    public float SpawnInterval { get; }
    public string[] EnemiesToSpawn { get; }

    public EnemyWave(float spawnTime, string[] enemiesToSpawn)
    {
        SpawnInterval = spawnTime / enemiesToSpawn.Length;
        EnemiesToSpawn = enemiesToSpawn;
    }
}