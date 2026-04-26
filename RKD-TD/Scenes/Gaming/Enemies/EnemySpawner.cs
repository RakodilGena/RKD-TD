using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using RKD_TD.Models.Interfaces;

namespace RKD_TD.Scenes.Gaming.Enemies;

internal sealed class EnemySpawner : IMyDrawable, IMyUpdatable
{
    private readonly EnemyFactory _enemyFactory;
    private readonly Queue<EnemyWave> _waves;
    private readonly float _wavesInterval;

    private EnemyWave _currentWave;
    private int _currentWaveIndex, _currentEnemyIndex;
    private float _spawnIntervalCounter, _wavesIntervalCounter;
    private bool _started;


    //todo: draw wave counter;


    public event EventHandler<Enemy>? EnemySpawned;
    public event EventHandler? AllWavesFinished;

    public EnemySpawner(
        Queue<EnemyWave> waves,
        float wavesInterval,
        EnemyFactory enemyFactory)
    {
        _waves = waves;
        _currentWave = _waves.Dequeue();

        _wavesInterval = wavesInterval;
        _enemyFactory = enemyFactory;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
    }

    public void Update(GameTime gameTime)
    {
        if (!_started)
            return;

        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_wavesIntervalCounter > 0)
        {
            bool setNextWave = HandleWavesInterval(deltaTime);
            if (setNextWave)
            {
                SetNextWave();
                //Console.WriteLine($"[{gameTime.TotalGameTime.TotalSeconds}] Set next wave!");
            }

            return;
        }


        var spawnEnemy = HandleSpawnInterval(deltaTime);
        if (!spawnEnemy)
            return;

        //Console.WriteLine($"[{gameTime.TotalGameTime.TotalSeconds}] Spawn enemy!");
        SpawnEnemy();
        PostSpawnSwitchToNextAction();
    }
    //какие кейсы.
    //если нет волны - ставим волну.

    private bool HandleWavesInterval(float deltaTime)
    {
        _wavesIntervalCounter -= deltaTime;

        if (_wavesIntervalCounter > 0)
            return false;

        _wavesIntervalCounter = 0;
        return true;
    }

    private void SetNextWave()
    {
        _currentWave = _waves.Dequeue();
        _currentWaveIndex++;
        _spawnIntervalCounter = _currentWave.SpawnInterval;
        _currentEnemyIndex = 0;
    }

    private bool HandleSpawnInterval(float deltaTime)
    {
        if (_spawnIntervalCounter is 0)
            return true;

        _spawnIntervalCounter -= deltaTime;
        if (_spawnIntervalCounter < 0)
        {
            _spawnIntervalCounter = 0;
        }

        return false;
    }

    private void SpawnEnemy()
    {
        var enemyAlias = _currentWave.EnemiesToSpawn[_currentEnemyIndex];

        Enemy enemy = _enemyFactory.CreateEnemy(enemyAlias);

        EnemySpawned?.Invoke(this, enemy);
    }

    private void PostSpawnSwitchToNextAction()
    {
        _currentEnemyIndex++;

        //more enemies to spawn.
        if (_currentEnemyIndex < _currentWave.EnemiesToSpawn.Length)
        {
            _spawnIntervalCounter = _currentWave.SpawnInterval;
            return;
        }

        //no enemies in wave, check if next wave exists
        var nextWaveExists = _waves.Count > 0;
        if (nextWaveExists)
        {
            _wavesIntervalCounter = _wavesInterval;
            return;
        }

        //no waves, game finished.
        _started = false;
        AllWavesFinished?.Invoke(this, EventArgs.Empty);
    }

    public void Start() => _started = true;

    public static EnemySpawner FromFile(
        XDocument mapDoc,
        TextureAtlas gameObjectsTextures)
    {
        var factory = EnemyFactory.FromFile(
            mapDoc,
            gameObjectsTextures);

        var root = mapDoc.Root!;
        var spawnerElement = root.Element("Spawner")!;
        var pauseBetweenWaves = float.Parse(spawnerElement.Attribute("pauseBetweenWaves")!.Value);

        var waveElements = spawnerElement
            .Elements("Waves")
            .Elements("Wave");

        Queue<EnemyWave> waves = [];
        List<string> enemiesToSpawn = [];
        foreach (var waveElement in waveElements)
        {
            var spawnTime = float.Parse(waveElement.Attribute("spawnTime")!.Value);
            var enemies = waveElement.Value.Split(";",
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var enemy in enemies)
            {
                var split = enemy.Split(':');
                var enemyAlias = split[0];
                var enemiesCount = int.Parse(split[1]);

                for (int i = 0; i < enemiesCount; i++)
                {
                    enemiesToSpawn.Add(enemyAlias);
                }
            }

            var wave = new EnemyWave(spawnTime, enemiesToSpawn.ToArray());
            waves.Enqueue(wave);
            enemiesToSpawn.Clear();
        }

        //todo: later use it for generator!
        //var totalWaves = int.Parse(spawnerElement.Attribute("totalWaves")!.Value);

        return new EnemySpawner(
            waves,
            pauseBetweenWaves,
            factory);
    }
}