using System;
using Microsoft.Xna.Framework;
using RKD_TD.Helpers;
using RKD_TD.Scenes.Gaming.Animations;
using RKD_TD.Scenes.Gaming.Projectiles;

namespace RKD_TD.Scenes.Gaming.ActiveTurrets;

internal sealed class TurretBarrel
{
    private readonly TurretFiringPoint[] _firingPoints;
    private readonly TurretFiringMode _firingMode;
    private readonly Vector2[] _gunFlashPoints;
    
    private readonly string _projectileAlias;
    private readonly string _flashAlias;

    private readonly ProjectileFactory _projectileFactory;
    private readonly GunShotFlashFactory _gunShotFlashFactory;

    private int _currentFiringPointIdx;

    public TurretBarrel(
        TurretFiringPoint[] firingPoints,
        TurretFiringMode firingMode, 
        Vector2[] gunFlashPoints,
        
        string projectileAlias,
        string flashAlias, 
        
        ProjectileFactory projectileFactory,
        GunShotFlashFactory gunShotFlashFactory)
    {
        _firingPoints = firingPoints;
        _projectileFactory = projectileFactory;
        _firingMode = firingMode;
        _projectileAlias = projectileAlias;
        _flashAlias = flashAlias;
        _gunShotFlashFactory = gunShotFlashFactory;
        _gunFlashPoints = gunFlashPoints;
    }

    public (Projectile[] projectiles, GunShotFlash[] flashes) Fire(
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
            var projectiles =  new Projectile[_firingPoints.Length];
            var flashes =  new GunShotFlash[_firingPoints.Length];

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

    private (Projectile projectile, GunShotFlash flash) CreateProjectileAndFlash(
        Vector2 turretCenter,
        float rotation,
        int pointIdx)
    {
        var firingPoint = _firingPoints[pointIdx];
        
        var firingPointAbsolutePosition = turretCenter + Vector2Helper.GetRotatedVector(
            firingPoint.Position,
            rotation);
        var bulletRotation = rotation + firingPoint.BulletExtraAngleInRadians;

        var projectile = _projectileFactory.Create(
            _projectileAlias,
            firingPointAbsolutePosition,
            bulletRotation);

        var flashPointAbsolutePosition = turretCenter + Vector2Helper.GetRotatedVector(
            _gunFlashPoints[pointIdx],
            rotation);
        
        var flash = _gunShotFlashFactory.Create(
            _flashAlias,
            flashPointAbsolutePosition,
            rotation);
        
        return (projectile, flash);
    }

    
}