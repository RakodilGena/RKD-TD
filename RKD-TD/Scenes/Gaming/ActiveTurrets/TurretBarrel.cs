using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Extensions;
using RKD_TD.Scenes.Gaming.Flashes;
using RKD_TD.Scenes.Gaming.Projectiles;

namespace RKD_TD.Scenes.Gaming.ActiveTurrets;

internal sealed class TurretBarrel
{
    private readonly TurretFiringPoint[] _firingPoints;
    private readonly TurretFiringMode _firingMode;
    private readonly Vector2[] _gunFlashPoints;

    private readonly ProjectileTemplate _projectileTemplate;
    private readonly string _flashAlias;

    private readonly ProjectileFactory _projectileFactory;
    private readonly FlashFactory _flashFactory;

    private int _currentFiringPointIdx;

    public TurretBarrel(
        TurretFiringPoint[] firingPoints,
        TurretFiringMode firingMode,
        Vector2[] gunFlashPoints,
        ProjectileTemplate projectileTemplate,
        string flashAlias,
        ProjectileFactory projectileFactory,
        FlashFactory flashFactory)
    {
        _firingPoints = firingPoints;
        _projectileFactory = projectileFactory;
        _firingMode = firingMode;
        _projectileTemplate = projectileTemplate;
        _flashAlias = flashAlias;
        _flashFactory = flashFactory;
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

        var projectile = _projectileFactory.Create(
            _projectileTemplate,
            firingPointAbsolutePosition,
            bulletRotation);

        var flashPointAbsolutePosition = turretCenter + _gunFlashPoints[pointIdx].GetRotatedVector(rotation);

        var flash = _flashFactory.Create(
            _flashAlias,
            flashPointAbsolutePosition,
            rotation);

        return (projectile, flash);
    }
}