using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Cameras;
using MonoGameLibrary.Graphics.Sprites;
using RKD_TD.Scenes.Gaming.Enemies;
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
        _firingDistanceSquared;


    private readonly TurretFiringPoint[] _firingPoints;
    private readonly string _projectileType;

    //todo: add ProjectileFactory


    private float _currentReloadTime;

    private float CurrentRotation
    {
        get;
        set
        {
            field = WrapAngle(value);
            _barrelSprite.Rotation = field;
        }
    }

    private Enemy? _fixatedEnemy;


    private readonly float _minIdleTimerSec = 1, _idleTimerMultiplierSec = 4;

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


    public Turret(
        Sprite barrelSprite,
        Sprite carriageSprite,
        Vector2 position,
        float rotationSpeedRadianInSec,
        float reloadTimeInSec,
        float fixateDistanceSquared,
        float firingDistanceSquared,
        TurretFiringPoint[] firingPoints,
        string projectileType,
        BuildCell occupiedCell)
    {
        _barrelSprite = barrelSprite;
        _position = position;
        _rotationSpeedRadianInSec = rotationSpeedRadianInSec;
        _reloadTimeInSec = reloadTimeInSec;
        _fixateDistanceSquared = fixateDistanceSquared;
        _firingDistanceSquared = firingDistanceSquared;
        _firingPoints = firingPoints;
        _projectileType = projectileType;
        OccupiedCell = occupiedCell;
        _carriageSprite = carriageSprite;
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
            if (_fixatedEnemy.State is not Enemy.EnemyState.Vulnerable)
            {
                Unfixate();
                return;
            }

            var vectorToEnemy = _fixatedEnemy.Target - _position;
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
                    var desiredAngle = MathF.Atan2(vectorToEnemy.Y, vectorToEnemy.X);
                    RotateTowards(
                        desiredAngle,
                        deltaSeconds,
                        out _);
                }

                return;
            }

            //can shoot, try it.
            var desiredAngle1 = MathF.Atan2(vectorToEnemy.Y, vectorToEnemy.X);
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

    private void HandleReload(float deltaSeconds)
    {
        if (_currentReloadTime > 0)
            _currentReloadTime -= deltaSeconds * _reloadTimeInSec;
    }

    private void Unfixate()
    {
        _fixatedEnemy = null;
        _idleRotationSpeedRadianInSec = 0f;
        _idleTimerSec = _minIdleTimerSec * 2;
    }

    private void Fixate(Enemy enemy) => _fixatedEnemy = enemy;

    private Enemy? FindClosestTargetWithin(float rangeSquared, HashSet<Enemy> enemies)
    {
        var minDistance = rangeSquared;
        Enemy? closestTarget = null;

        foreach (var enemy in enemies)
        {
            if (enemy.State is not Enemy.EnemyState.Vulnerable)
                continue;

            var distanceToEnemy = (enemy.Target - _position).LengthSquared();

            if (minDistance < distanceToEnemy)
                continue;

            minDistance = distanceToEnemy;
            closestTarget = enemy;
        }

        return closestTarget;
    }

    private static float WrapAngle(float angle)
    {
        float twoPi = MathHelper.TwoPi;
        return (angle % twoPi + twoPi) % twoPi;
    }

    private void RotateTowards(float desiredAngle, float deltaSeconds, out bool reached)
    {
        float desired = WrapAngle(desiredAngle);

        float diff = desired - CurrentRotation;

        // now wrap diff to (-Pi, Pi)
        if (diff > MathHelper.Pi) diff -= MathHelper.TwoPi;
        if (diff < -MathHelper.Pi) diff += MathHelper.TwoPi;

        float maxStep = _rotationSpeedRadianInSec * deltaSeconds;
        float newAngle;
        if (MathF.Abs(diff) <= maxStep)
        {
            newAngle = desiredAngle;
            reached = true;
        }
        else
        {
            newAngle = CurrentRotation + MathF.Sign(diff) * maxStep;
            reached = false;
        }

        CurrentRotation = newAngle;
    }

    private void Fire(float deltaSeconds)
    {
        _barrelSprite.Update(deltaSeconds);

        if (_currentReloadTime > 0)
            return;


        Console.WriteLine("SHOOTING!");
        //todo implement shooting
        _currentReloadTime += _reloadTimeInSec;
    }

    private void HandleIdleRotation(float deltaSeconds)
    {
        if (_idleTimerSec <= 0)
        {
            //resetting idle rotation
            _idleTimerSec =
                (float)Random.Shared.NextDouble() * _idleTimerMultiplierSec + _minIdleTimerSec; //from 1 to 4

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