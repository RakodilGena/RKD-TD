using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Graphics.Labels;
using RKD_TD.Assets;

namespace RKD_TD.Scenes.Gaming.Enemies.Spawns;

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


    private readonly Label _waveTextLabel;
    private readonly Label _waveCounterLabel;


    public event EventHandler<Enemy[]>? EnemySpawned;
    public event EventHandler<int>? WaveFinished;
    public event EventHandler? AllWavesFinished;


    public EnemySpawner(
        Queue<EnemyWave> waves,
        float wavesInterval,
        EnemyFactory enemyFactory,
        Vector2 widgetPosition,
        SpriteFont waveTextFont,
        SpriteFont waveCounterFont)
    {
        _waves = waves;
        _maxWaves = waves.Count;

        _wavesInterval = wavesInterval;
        _enemyFactory = enemyFactory;

        _waveTextLabel = CreateTextLabel(widgetPosition, waveTextFont, out var counterLabelPosition);
        _waveCounterLabel = CreateCounterLabel(counterLabelPosition, waveCounterFont);

        _currentWaveIndex = -1;

        SetNextWave();

        _paused = true;
    }

    private static Label CreateTextLabel(
        Vector2 labelPosition,
        SpriteFont font,
        out Vector2 counterLabelPosition)
    {
        const string text = "Round";
        var label = new BorderedLabel(text, font)
        {
            Position = labelPosition,
            Color = Color.White,
            BorderColor = Color.Black,
            BorderWidth = new Vector2(2f)
        };

        var size = font.MeasureString(text);

        counterLabelPosition = new Vector2(labelPosition.X + size.X, labelPosition.Y);

        return label;
    }

    private static Label CreateCounterLabel(
        Vector2 labelPosition,
        SpriteFont font)
    {
        labelPosition += new Vector2(20, -2);

        var label = new BorderedLabel(font)
        {
            Position = labelPosition,
            Color = Color.White,
            BorderColor = Color.Black,
            BorderWidth = new Vector2(2f)
        };

        return label;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _waveTextLabel.Draw(spriteBatch);
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

        string labelText = $"{_currentWaveIndex + 1}/{_maxWaves}";
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

        Enemy[] enemies = _enemyFactory.CreateEnemy(
            enemyAlias,
            _currentWaveIndex);

        _currentEnemyIndex++;

        EnemySpawned?.Invoke(this, enemies);

        //more enemies to spawn.
        var waveHasMoreEnemies = _currentEnemyIndex < _currentWave.EnemiesToSpawn.Length;
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
            WaveFinished?.Invoke(this, _currentWave.Reward);
            return;
        }

        //no waves, game finished.
        _done = true;
        AllWavesFinished?.Invoke(this, EventArgs.Empty);
    }

    public void Resume() => _paused = false;

    public static EnemySpawner FromFile(
        XDocument mapDoc,
        XDocument enemyConfigDoc,
        XDocument healthBarConfigDoc,
        TextureAtlas gameObjectsTextures,
        Vector2 widgetPosition)
    {
        var factory = EnemyFactory.FromFile(
            mapDoc,
            enemyConfigDoc,
            healthBarConfigDoc,
            gameObjectsTextures);

        var root = mapDoc.Root!;
        var spawnerElement = root.Element("Spawner")!;

        var basicWaveReward = int.Parse(spawnerElement.Attribute("basicWaveReward")!.Value);
        var waveRewardIncrease = int.Parse(spawnerElement.Attribute("waveRewardIncrease")!.Value);

        var totalWaves = int.Parse(spawnerElement.Attribute("totalWaves")!.Value);
        var pauseBetweenWaves = float.Parse(spawnerElement.Attribute("pauseBetweenWaves")!.Value);
        var basicSpawnInterval = float.Parse(spawnerElement.Attribute("basicSpawnInterval")!.Value);
        var spawnIntervalDecrease = float.Parse(spawnerElement.Attribute("spawnIntervalDecrease")!.Value);

        var initialWaveValue = int.Parse(spawnerElement.Attribute("initialWaveValue")!.Value);
        var waveValueIncrease = int.Parse(spawnerElement.Attribute("waveValueIncrease")!.Value);
        var waveValueMultiplier = float.Parse(spawnerElement.Attribute("waveValueMultiplier")!.Value);

        var bossWavesValue = spawnerElement.Attribute("bossWaves")!.Value;
        var bossWaves = bossWavesValue
            .Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(bossWave =>
                bossWave.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Select(bossWave =>
            {
                int waveIdx = int.Parse(bossWave[0]);
                bool hasChaff = bossWave.Length > 1 && bossWave[1] is "has chaff";

                return new BossWave(waveIdx, hasChaff);
            }).ToArray();


        Queue<EnemyWave> waves = [];
        List<string> enemiesToSpawn = [];
        var reward = basicWaveReward;
        int waveNumber = 0;


        var waveElements = spawnerElement.Element("Waves")!.Elements("Wave");
        foreach (var waveElement in waveElements)
        {
            var enemies = waveElement.Value.Split("\n",
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
                enemiesToSpawn.ToArray(),
                reward,
                waveNumber,
                basicSpawnInterval,
                spawnIntervalDecrease);

            reward += waveRewardIncrease;
            waveNumber++;

            waves.Enqueue(wave);
            enemiesToSpawn.Clear();
        }

        var enemyTemplates = factory.GetEnemyTemplates();
        var generator = new EnemyWavesGenerator(
            initialWaveValue,
            waveValueMultiplier,
            waveValueIncrease,
            basicWaveReward,
            waveRewardIncrease,
            basicSpawnInterval,
            spawnIntervalDecrease,
            enemyTemplates,
            bossWaves);

        var generatedWaves = generator.GenerateAllWaves(
            startingWaveIndex: waveNumber,
            totalWaves);

        foreach (var wave in generatedWaves)
        {
            waves.Enqueue(wave);
        }

        var enemiesPicked = waves.SelectMany(w => w.EnemiesToSpawn)
            .GroupBy(w => w)
            .Select(w => (w.Key, w.Count())).ToArray();


        var textFont = GlobalAssets.FontAtlas.GetFont(Fonts.USER_RESOURCES);
        var counterFont = GlobalAssets.FontAtlas.GetFont(Fonts.USER_RESOURCES_DIGITS);

        return new EnemySpawner(
            waves,
            pauseBetweenWaves,
            factory,
            widgetPosition,
            textFont,
            counterFont);
    }
}