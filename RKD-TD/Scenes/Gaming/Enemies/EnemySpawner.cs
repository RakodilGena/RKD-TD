using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using RKD_TD.Assets;

namespace RKD_TD.Scenes.Gaming.Enemies;

internal sealed class EnemySpawner
{
    private readonly EnemyFactory _enemyFactory;
    private readonly Queue<EnemyWave> _waves;
    private readonly float _wavesInterval;
    private readonly int _maxWaves;

    private EnemyWave _currentWave = null!;
    private int _currentWaveIndex, _currentEnemyIndex;
    private float _spawnIntervalCounter, _wavesIntervalCounter;
    private bool _paused, _done;


    private readonly Label _waveCounterLabel;


    public event EventHandler<Enemy>? EnemySpawned;
    public event EventHandler<int>? WaveFinished;
    public event EventHandler? AllWavesFinished;


    public EnemySpawner(
        Queue<EnemyWave> waves,
        float wavesInterval,
        EnemyFactory enemyFactory,
        Label waveCounterLabel)
    {
        _waves = waves;
        _maxWaves = waves.Count;

        _wavesInterval = wavesInterval;
        _enemyFactory = enemyFactory;

        _waveCounterLabel = waveCounterLabel;

        SetNextWave();

        _paused = true;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _waveCounterLabel.Draw(spriteBatch);
    }

    public void Update(float deltaSeconds)
    {
        if (_paused || _done)
            return;

        if (_wavesIntervalCounter > 0)
        {
            bool setNextWave = HandleWavesInterval(deltaSeconds);
            if (setNextWave)
            {
                SetNextWave();
            }

            return;
        }


        var spawnEnemy = HandleSpawnInterval(deltaSeconds);
        if (!spawnEnemy)
            return;

        SpawnEnemy();
    }

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

        string labelText = $"Wave {_currentWaveIndex} of {_maxWaves}";
        _waveCounterLabel.Text = labelText;
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

        _currentEnemyIndex++;

        var waveHasMoreEnemies = _currentEnemyIndex < _currentWave.EnemiesToSpawn.Length;
        if (!waveHasMoreEnemies)
        {
            //if enemy is the last, stop spawn and wait for it to die/reach destination.
            enemy.Destroyed += OnLastEnemyOfWaveDestroyed;
            enemy.ReachedPortal += OnLastEnemyOfWaveDestroyed;
        }

        EnemySpawned?.Invoke(this, enemy);

        //more enemies to spawn.
        if (waveHasMoreEnemies)
        {
            _spawnIntervalCounter = _currentWave.SpawnInterval;
            return;
        }

        //no enemies in wave, check if next wave exists
        var nextWaveExists = _waves.Count > 0;
        if (nextWaveExists)
        {
            _wavesIntervalCounter = _wavesInterval;
            _paused = true;
            return;
        }

        //no waves, game finished.
        _done = true;
    }

    private void OnLastEnemyOfWaveDestroyed(object? sender, int discarded)
    {
        if (!_done)
            WaveFinished?.Invoke(this, _currentWave.Reward);

        else
            AllWavesFinished?.Invoke(this, EventArgs.Empty);

        _paused = false;

        // var enemy = (Enemy)sender!;
        // enemy.Destroyed -= OnLastEnemyOfWaveDestroyed;
        // enemy.ReachedPortal -= OnLastEnemyOfWaveDestroyed;
    }

    public void Start() => _paused = false;

    public static EnemySpawner FromFile(
        XDocument mapDoc,
        TextureAtlas gameObjectsTextures,
        Vector2 labelPosition)
    {
        var factory = EnemyFactory.FromFile(
            mapDoc,
            gameObjectsTextures);

        var root = mapDoc.Root!;
        var spawnerElement = root.Element("Spawner")!;
        var pauseBetweenWaves = float.Parse(spawnerElement.Attribute("pauseBetweenWaves")!.Value);

        var wavesElement = spawnerElement.Element("Waves")!;
        var basicReward = int.Parse(wavesElement.Attribute("basicReward")!.Value);
        var rewardLevelMultiplier = int.Parse(wavesElement.Attribute("rewardLevelMultiplier")!.Value);

        var waveElements = wavesElement.Elements("Wave");

        Queue<EnemyWave> waves = [];
        List<string> enemiesToSpawn = [];
        var reward = basicReward;
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

            var wave = new EnemyWave(
                spawnTime,
                enemiesToSpawn.ToArray(),
                reward);

            reward += rewardLevelMultiplier;

            waves.Enqueue(wave);
            enemiesToSpawn.Clear();
        }

        //todo: later use it for generator!
        //var totalWaves = int.Parse(spawnerElement.Attribute("totalWaves")!.Value);


        var font = GlobalAssets.FontAtlas.GetFont(Fonts.USER_RESOURCES);
        var label = new Label(font)
        {
            Position = labelPosition
        };

        return new EnemySpawner(
            waves,
            pauseBetweenWaves,
            factory,
            label);
    }
}