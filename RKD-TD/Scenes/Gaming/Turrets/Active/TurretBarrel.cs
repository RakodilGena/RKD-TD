using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Extensions;
using RKD_TD.Scenes.Gaming.Flashes;
using RKD_TD.Scenes.Gaming.Projectiles;

namespace RKD_TD.Scenes.Gaming.Turrets.Active;

internal sealed class TurretBarrel
{
    private readonly TurretFiringPoint[] _firingPoints;
    private readonly TurretFiringMode _firingMode;
    private readonly Vector2[] _gunFlashPoints;

    private readonly float[] _projectileFlightRange;
    private readonly int[] _directDamage;
    private readonly int _aoeRange;
    private readonly int[]? _aoeDamage;

    private readonly ProjectileTemplate _projectileTemplate;
    private readonly string _flashAlias;

    private readonly ProjectileFactory _projectileFactory;
    private readonly FlashFactory _flashFactory;

    private int _currentFiringPointIdx;

    private int _level;

    public TurretBarrel(
        int level,
        TurretFiringPoint[] firingPoints,
        TurretFiringMode firingMode,
        Vector2[] gunFlashPoints,
        ProjectileTemplate projectileTemplate,
        float[] projectileFlightRange,
        int[] directDamage,
        int aoeRange,
        int[]? aoeDamage,
        string flashAlias,
        ProjectileFactory projectileFactory,
        FlashFactory flashFactory)
    {
        _level = level;
        _firingPoints = firingPoints;
        _projectileFactory = projectileFactory;
        _firingMode = firingMode;
        _projectileTemplate = projectileTemplate;
        _flashAlias = flashAlias;
        _flashFactory = flashFactory;
        _projectileFlightRange = projectileFlightRange;
        _directDamage = directDamage;
        _aoeRange = aoeRange;
        _aoeDamage = aoeDamage;
        _gunFlashPoints = gunFlashPoints;
    }


    public (Projectile[] projectiles, Flash[] flashes) Fire(
        Vector2 turretCenter,
        float rotation)
    {
        if (_firingMode is TurretFiringMode.Single)
        {
            var (projectile, flash) = CreateProjectileAndFlash(
                turretCenter,
                rotation,
                pointIdx: 0);

            return ([projectile], [flash]);
        }

        if (_firingMode is TurretFiringMode.Random)
        {
            var idx = Random.Shared.Next(_firingPoints.Length);

            var (projectile, flash) = CreateProjectileAndFlash(
                turretCenter,
                rotation,
                idx);

            return ([projectile], [flash]);
        }

        if (_firingMode is TurretFiringMode.Rotation)
        {
            var (projectile, flash) = CreateProjectileAndFlash(
                turretCenter,
                rotation,
                _currentFiringPointIdx);

            _currentFiringPointIdx++;
            if (_currentFiringPointIdx >= _firingPoints.Length)
                _currentFiringPointIdx = 0;

            return ([projectile], [flash]);
        }

        {
            //all
            var projectiles = new Projectile[_firingPoints.Length];
            var flashes = new Flash[_firingPoints.Length];

            for (int pointIdx = 0; pointIdx < _firingPoints.Length; pointIdx++)
            {
                var (projectile, flash) = CreateProjectileAndFlash(
                    turretCenter,
                    rotation,
                    pointIdx);

                projectiles[pointIdx] = projectile;
                flashes[pointIdx] = flash;
            }

            return (projectiles, flashes);
        }
    }

    private (Projectile projectile, Flash flash) CreateProjectileAndFlash(
        Vector2 turretCenter,
        float rotation,
        int pointIdx)
    {
        var firingPoint = _firingPoints[pointIdx];

        var firingPointAbsolutePosition = turretCenter + firingPoint.Position.GetRotatedVector(rotation);
        var bulletRotation = rotation + firingPoint.BulletExtraAngleInRadians;

        var projectileValues = new ProjectileValues(
            _projectileFlightRange[_level],
            _directDamage[_level],
            _aoeRange,
            _aoeDamage?[_level] ?? 0);

        var projectile = _projectileFactory.Create(
            _projectileTemplate,
            projectileValues,
            firingPointAbsolutePosition,
            bulletRotation);

        var flashPointAbsolutePosition = turretCenter + _gunFlashPoints[pointIdx].GetRotatedVector(rotation);

        var flash = _flashFactory.Create(
            _flashAlias,
            flashPointAbsolutePosition,
            rotation);

        return (projectile, flash);
    }

    public void Upgrade(int level)
    {
        _level = level;
    }
}