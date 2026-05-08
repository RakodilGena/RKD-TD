using System;
using System.Linq;
using Microsoft.Xna.Framework;
using RKD_TD.Scenes.Gaming.Projectiles;

namespace RKD_TD.Scenes.Gaming.ActiveTurrets;

internal sealed class TurretBarrel
{
    private readonly TurretFiringPoint[] _firingPoints;
    private readonly ProjectileFactory _projectileFactory;
    private readonly TurretFiringMode _firingMode;
    private readonly string _projectileAlias;

    private int _currentFiringPointIdx;

    public TurretBarrel(
        TurretFiringPoint[] firingPoints,
        ProjectileFactory projectileFactory,
        TurretFiringMode firingMode,
        string projectileAlias)
    {
        _firingPoints = firingPoints;
        _projectileFactory = projectileFactory;
        _firingMode = firingMode;
        _projectileAlias = projectileAlias;
    }

    public Projectile[] CreateProjectiles(
        Vector2 position,
        float rotation)
    {
        if (_firingMode is TurretFiringMode.Single)
        {
            var projectile = CreateProjectile(position, rotation, _firingPoints[0]);
            return [projectile];
        }

        if (_firingMode is TurretFiringMode.Random)
        {
            var idx = Random.Shared.Next(_firingPoints.Length);
            var projectile = CreateProjectile(position, rotation, _firingPoints[idx]);
            return [projectile];
        }

        if (_firingMode is TurretFiringMode.Rotation)
        {
            var projectile = CreateProjectile(position, rotation, _firingPoints[_currentFiringPointIdx]);

            _currentFiringPointIdx++;
            if (_currentFiringPointIdx >= _firingPoints.Length)
                _currentFiringPointIdx = 0;

            return [projectile];
        }

        //all
        return _firingPoints.Select(fp => CreateProjectile(position, rotation, fp)).ToArray();
    }

    private Projectile CreateProjectile(
        Vector2 position,
        float rotation,
        TurretFiringPoint firingPoint)
    {
        var firingPointAbsolutePosition = position + GetCurrentFiringPointPosition(
            firingPoint.Position,
            rotation);

        var bulletRotation = rotation - firingPoint.BulletExtraAngleInRadians;

        return _projectileFactory.Create(
            _projectileAlias,
            firingPointAbsolutePosition,
            bulletRotation);
    }

    private static Vector2 GetCurrentFiringPointPosition(
        Vector2 firingPoint,
        float rotation)
    {
        const float halfPi = MathF.PI * 0.5f;
        float currentAngle = MathF.Atan2(firingPoint.Y, firingPoint.X);
        float newAngle = currentAngle + rotation - halfPi;
        return new Vector2(MathF.Cos(newAngle), MathF.Sin(newAngle)) * firingPoint.Length();
    }
}