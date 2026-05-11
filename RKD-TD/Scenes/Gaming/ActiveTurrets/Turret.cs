using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Cameras;
using MonoGameLibrary.Extensions;
using MonoGameLibrary.Graphics.Sprites;
using RKD_TD.Scenes.Gaming.Enemies;
using RKD_TD.Scenes.Gaming.Flashes;
using RKD_TD.Scenes.Gaming.Projectiles;
using RKD_TD.Scenes.Gaming.PurchaseTurrets;

namespace RKD_TD.Scenes.Gaming.ActiveTurrets;

internal sealed class Turret
{
    private readonly Sprite _barrelSprite, _carriageSprite;
    private readonly Vector2 _position;

    private readonly float
        _rotationSpeedRadianInSec,
        _reloadTimeInSec,
        _fixateDistanceSquared,
        _firingDistance,
        _firingDistanceSquared;

    private readonly TurretBarrel _turretBarrel;

    private readonly TurretAimingMode _aimingMode;

    //these three values required for 'predictive' aiming mode
    private readonly float _projectileFlightSpeed;
    private readonly int _barrelLenght;


    private float _currentReloadTime;

    private float CurrentRotation
    {
        get;
        set
        {
            var wrapped = value.WrapAngle();
            field = wrapped;
            _barrelSprite.Rotation = wrapped;
        }
    }

    private Enemy? _fixatedEnemy;


    private const float MIN_IDLE_TIMER_SEC = 1;
    private const float IDLE_TIMER_MULTIPLIER_SEC = 4;

    private float
        _idleRotationSpeedRadianInSec,
        _idleTimerSec;

    public ICamera? Camera
    {
        get;
        set
        {
            _barrelSprite.Camera = value;
            _carriageSprite.Camera = value;
            field = value;
        }
    }

    public BuildCell OccupiedCell { get; private set; }


    public event EventHandler<TurretShotEventArgs>? ProjectilesFired;

    public Turret(
        Sprite barrelSprite,
        Sprite carriageSprite,
        Vector2 position,
        float rotationSpeedRadianInSec,
        float reloadTimeInSec,
        float fixateDistanceSquared,
        float firingDistance,
        float firingDistanceSquared,
        TurretFiringPoint[] firingPoints,
        TurretFiringMode firingMode,
        Vector2[] gunFlashPoints,
        TurretAimingMode aimingMode,
        int barrelLenght,
        string projectileAlias,
        string flashAlias,
        BuildCell occupiedCell,
        ProjectileFactory projectileFactory,
        FlashFactory flashFactory)
    {
        _barrelSprite = barrelSprite;
        _position = position;
        _rotationSpeedRadianInSec = rotationSpeedRadianInSec;
        _reloadTimeInSec = reloadTimeInSec;
        _fixateDistanceSquared = fixateDistanceSquared;
        _firingDistanceSquared = firingDistanceSquared;
        OccupiedCell = occupiedCell;
        _firingDistance = firingDistance;
        _carriageSprite = carriageSprite;

        var projectileTemplate = projectileFactory.GetTemplate(projectileAlias);

        _turretBarrel = new TurretBarrel(
            firingPoints,
            firingMode,
            gunFlashPoints,
            projectileTemplate,
            flashAlias,
            projectileFactory,
            flashFactory);


        _aimingMode = aimingMode;
        _barrelLenght = barrelLenght;
        _projectileFlightSpeed = projectileTemplate.Speed;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _carriageSprite.Draw(spriteBatch, _position);
        _barrelSprite.Draw(spriteBatch, _position);
    }

    public void Update(
        float deltaSeconds,
        HashSet<Enemy> enemies)
    {
        HandleReload(deltaSeconds);

        //if enemy fixated - check distance.
        if (_fixatedEnemy is not null)
        {
            if (_fixatedEnemy.State is not EnemyState.Vulnerable)
            {
                Unfixate();
                return;
            }

            var vectorToEnemy = CalculateEnemyTarget(_fixatedEnemy) - _position;
            var distanceToEnemySquared = vectorToEnemy.LengthSquared();
            if (distanceToEnemySquared > _fixateDistanceSquared)
            {
                //too far away
                Unfixate();
                return;
            }

            if (distanceToEnemySquared > _firingDistanceSquared)
            {
                //cant shoot yet, try switching
                var newTarget = FindClosestTargetWithin(_firingDistanceSquared, enemies);
                if (newTarget is not null)
                {
                    Fixate(newTarget);
                }
                else
                {
                    var desiredAngle = vectorToEnemy.ToAngle();
                    RotateTowards(
                        desiredAngle,
                        deltaSeconds,
                        out _);
                }

                return;
            }

            //can shoot, try it.
            var desiredAngle1 = vectorToEnemy.ToAngle();
            RotateTowards(
                desiredAngle1,
                deltaSeconds,
                out var reached);

            if (reached)
                Fire(deltaSeconds);

            return;
        }

        var newTarget1 = FindClosestTargetWithin(_fixateDistanceSquared, enemies);
        if (newTarget1 is not null)
        {
            Fixate(newTarget1);
            return;
        }

        HandleIdleRotation(deltaSeconds);
    }

    private Vector2 CalculateEnemyTarget(Enemy enemy)
    {
        if (_aimingMode is TurretAimingMode.Predictive)
        {
            var distance = (enemy.Target - _position).Length() - _barrelLenght;

            if (distance < 0)
                return enemy.Target;

            var secondsToEnemy = distance / _projectileFlightSpeed;

            var unitVector = enemy.GetDirectionUnitVector();
            var enemyPositionWhenHit = enemy.Target + enemy.Speed * unitVector * secondsToEnemy;

            return enemyPositionWhenHit;
        }

        return enemy.Target;
    }

    private void HandleReload(float deltaSeconds)
    {
        if (_currentReloadTime > 0)
            _currentReloadTime -= deltaSeconds;
    }

    private void Unfixate()
    {
        _fixatedEnemy = null;
        _idleRotationSpeedRadianInSec = 0f;
        _idleTimerSec = MIN_IDLE_TIMER_SEC * 2;
    }

    private void Fixate(Enemy enemy) => _fixatedEnemy = enemy;

    private Enemy? FindClosestTargetWithin(float rangeSquared, HashSet<Enemy> enemies)
    {
        var minDistance = rangeSquared;
        Enemy? closestTarget = null;

        foreach (var enemy in enemies)
        {
            if (enemy.State is not EnemyState.Vulnerable)
                continue;

            var distanceToEnemy = (enemy.Target - _position).LengthSquared();

            if (minDistance < distanceToEnemy)
                continue;

            minDistance = distanceToEnemy;
            closestTarget = enemy;
        }

        return closestTarget;
    }

    private void RotateTowards(float desiredAngle, float deltaSeconds, out bool reached)
    {
        var newAngle = CurrentRotation.RotateTowards(
            desiredAngle,
            _rotationSpeedRadianInSec,
            deltaSeconds,
            out reached);
        CurrentRotation = newAngle;
    }

    private void Fire(float deltaSeconds)
    {
        _barrelSprite.Update(deltaSeconds);

        if (_currentReloadTime > 0)
            return;

        var (projectiles, flashes) = _turretBarrel.Fire(_position, CurrentRotation);

        foreach (var projectile in projectiles)
        {
            if (projectile is HomingMissile homingMissile)
                homingMissile.SetTarget(_fixatedEnemy!);
        }

        ProjectilesFired?.Invoke(this, new TurretShotEventArgs(projectiles, flashes));

        _currentReloadTime += _reloadTimeInSec;
    }

    private void HandleIdleRotation(float deltaSeconds)
    {
        if (_idleTimerSec <= 0)
        {
            //resetting idle rotation
            _idleTimerSec =
                (float)Random.Shared.NextDouble() * IDLE_TIMER_MULTIPLIER_SEC + MIN_IDLE_TIMER_SEC; //from 1 to 4

            //was afk, add angle
            if (_idleRotationSpeedRadianInSec is 0f)
            {
                var speed = (float)Random.Shared.NextDouble() * _rotationSpeedRadianInSec / 3
                            + _rotationSpeedRadianInSec / 6; // from 1/6 of the targetingSpeed to 1/2 of it

                var sign = Random.Shared.Next(2) is 0 ? -1 : 1;

                _idleRotationSpeedRadianInSec = speed * sign;
            }
            else
            {
                //was rotating, set afk
                _idleRotationSpeedRadianInSec = 0f;
            }

            return;
        }

        //keep the same rotation
        _idleTimerSec -= deltaSeconds;

        if (_idleRotationSpeedRadianInSec is 0f)
            return;

        CurrentRotation += _idleRotationSpeedRadianInSec * deltaSeconds;
    }

    /// <summary>
    /// MUST UNSUBSCRIBE IF TURRET IS REMOVED.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="removedEnemy"></param>
    public void OnEnemyRemoved(object? sender, Enemy removedEnemy)
    {
        if (_fixatedEnemy == removedEnemy)
            Unfixate();
    }
}