using System;
using System.Collections.Generic;
using System.Linq;

namespace RKD_TD.Scenes.Gaming.Enemies.Spawns;

internal sealed class EnemyWavesGenerator
{
    private readonly int _initialWaveValue;
    private readonly float _waveValueMultiplier;
    private readonly int _waveValueIncrease;
    private readonly int _basicWaveReward;
    private readonly int _waveRewardIncrease;
    private readonly float _basicSpawnInterval;
    private readonly float _spawnIntervalDecrease;
    private readonly List<EnemyTemplate> _regularEnemies;
    private readonly EnemyTemplate _boss;
    private readonly BossWave[] _bossWaves;

    public EnemyWavesGenerator(
        int initialWaveValue,
        float waveValueMultiplier,
        int waveValueIncrease,
        int basicWaveReward,
        int waveRewardIncrease,
        float basicSpawnInterval,
        float spawnIntervalDecrease,
        IReadOnlyList<EnemyTemplate> allEnemies,
        BossWave[] bossWaves)
    {
        _initialWaveValue = initialWaveValue;
        _waveValueMultiplier = waveValueMultiplier;
        _waveValueIncrease = waveValueIncrease;
        _basicWaveReward = basicWaveReward;
        _waveRewardIncrease = waveRewardIncrease;
        _basicSpawnInterval = basicSpawnInterval;
        _spawnIntervalDecrease = spawnIntervalDecrease;
        _bossWaves = bossWaves;

        _regularEnemies = allEnemies.Where(e => !e.IsBoss).ToList();
        _boss = allEnemies.Single(e => e.IsBoss);
    }

    public List<EnemyWave> GenerateAllWaves(
        int startingWaveIndex,
        int totalWaves)
    {
        List<EnemyWave> result = [];

        for (int i = startingWaveIndex; i < totalWaves; i++)
            result.Add(GenerateWave(i));

        return result;
    }

    private float CalculateWaveValue(int waveIndex)
    {
        return _initialWaveValue * MathF.Pow(_waveValueMultiplier, waveIndex) + _waveValueIncrease * waveIndex;
    }

    private int CalculateReward(int waveIndex)
    {
        return _basicWaveReward + _waveRewardIncrease * waveIndex;
    }

    private EnemyWave GenerateWave(int waveIndex)
    {
        var spawns = new List<string>();
        BossWave? bossWave = Array.Find(_bossWaves, b => b.WaveIndex == waveIndex);

        var waveValue = CalculateWaveValue(waveIndex);

        if (bossWave != null)
        {
            spawns.Add(_boss.Alias);

            if (bossWave.HasChaff)
            {
                float remainingBudget = waveValue - _boss.Cost;
                if (remainingBudget > 0)
                    spawns.AddRange(FillWithEnemies(remainingBudget));
            }
        }
        else
        {
            spawns.AddRange(FillWithEnemies(waveValue));
        }

        var reward = CalculateReward(waveIndex);

        var wave = new EnemyWave(
            spawns.ToArray(),
            reward,
            waveIndex,
            _basicSpawnInterval,
            _spawnIntervalDecrease);

        return wave;
    }

    private List<string> FillWithEnemies(float budget)
    {
        var spawns = new List<string>();
        float remaining = budget;

        const float costPower = 1.1f;

        while (remaining > 0)
        {
            // only consider enemies we can still afford
            var affordable = _regularEnemies.Where(e => e.Cost <= remaining).ToArray();
            if (affordable.Length == 0) break;

            // weight is inverted cost - cheaper enemies picked more often
            float totalWeight = affordable.Sum(e => 1f / MathF.Pow(e.Cost, costPower));
            float roll = (float)Random.Shared.NextDouble() * totalWeight;

            EnemyTemplate? picked = null;
            foreach (var enemy in affordable)
            {
                roll -= 1f / MathF.Pow(enemy.Cost, costPower);
                if (roll <= 0)
                {
                    picked = enemy;
                    break;
                }
            }

            picked ??= affordable[^1]; // fallback for floating point edge case

            spawns.Add(picked.Alias);
            remaining -= picked.Cost;
        }

        return spawns;
    }
}