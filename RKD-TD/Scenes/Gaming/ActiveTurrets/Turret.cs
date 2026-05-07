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
            field = value;
            _barrelSprite.Rotation = value;
        }
    }

    private Enemy? _fixatedEnemy;
    //private float _idleFixateAngle;

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

        //todo cant find anything, imitate idle rotation
        // RotateTowards(_idleFixateAngle, deltaSeconds, out var reachedIdleAngle);
        // if (reachedIdleAngle)
        // {
        //     _idleFixateAngle = MathHelper.ToRadians(Random.Shared.Next(-360, 360));
        // }
    }

    private void HandleReload(float deltaSeconds)
    {
        if (_currentReloadTime > 0)
            _currentReloadTime -= deltaSeconds * _reloadTimeInSec;
    }

    private void Unfixate() => _fixatedEnemy = null;

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

    private void RotateTowards(float desiredAngle, float deltaSeconds, out bool reached)
    {
        float diff = desiredAngle - CurrentRotation;

        // wrap diff to (-Pi, Pi) so we always get the shortest arc
        diff = (diff + MathHelper.Pi) % MathHelper.TwoPi - MathHelper.Pi;

        float maxStep = _rotationSpeedRadianInSec * deltaSeconds;

        float newRotation;
        if (MathF.Abs(diff) <= maxStep)
        {
            newRotation = desiredAngle;
            reached = true;
        }
        else
        {
            newRotation = CurrentRotation + MathF.Sign(diff) * maxStep;
            reached = false;
        }

        // keep _currentAngle in (-Pi, Pi)
        CurrentRotation = (newRotation + MathHelper.Pi) % MathHelper.TwoPi - MathHelper.Pi;
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