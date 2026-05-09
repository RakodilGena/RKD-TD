using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Cameras;
using MonoGameLibrary.Collisions;
using MonoGameLibrary.Graphics.Sprites;
using RKD_TD.Scenes.Gaming.Enemies;
using RKD_TD.Scenes.Gaming.Explosions;

namespace RKD_TD.Scenes.Gaming.Projectiles;

internal class Projectile
{
    private readonly Sprite _sprite;

    private readonly string _explosionAlias;
    private readonly ExplosionFactory _explosionFactory;

    protected float Rotation
    {
        get;
        set
        {
            field = value;
            _sprite.Rotation = value;
        }
    }

    private readonly float
        _speed,
        _flightRange;

    private readonly int _directDamage, _aoeRange, _aoeDamage, _hitCircleRadius;

    private Vector2 _position;

    private float _pathTraveled;
    private bool _dead;

    public ICamera? Camera
    {
        get;
        set
        {
            _sprite.Camera = value;
            field = value;
        }
    }

    public event EventHandler? Exhausted;
    public event EventHandler<Explosion>? Collided;

    public Projectile(
        Sprite sprite,
        float speed,
        float flightRange,
        int hitCircleRadius,
        int directDamage,
        int aoeDamage,
        int aoeRange,
        Vector2 position,
        float rotation,
        string explosionAlias,
        ExplosionFactory explosionFactory)
    {
        _sprite = sprite;
        _speed = speed;
        _flightRange = flightRange;
        _directDamage = directDamage;
        _aoeDamage = aoeDamage;
        _aoeRange = aoeRange;
        Rotation = rotation;
        _explosionAlias = explosionAlias;
        _explosionFactory = explosionFactory;
        _hitCircleRadius = hitCircleRadius;
        _position = position;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _sprite.Draw(spriteBatch, _position);

        if (GameCore.DRAW_HIT_BOX)
#pragma warning disable CS0162 // Unreachable code detected
        {
            Circle.DrawHitCircle(spriteBatch, Camera, _position, _hitCircleRadius, Color.BlueViolet);
        }
#pragma warning restore CS0162 // Unreachable code detected
    }

    public void Update(
        HashSet<Enemy> enemies,
        float deltaSeconds)
    {
        if (_dead)
            return;

        if (HandleExhausted())
            return;

        if (HandleHit(enemies))
            return;

        HandleMove(deltaSeconds);
    }

    private bool HandleExhausted()
    {
        if (_pathTraveled < _flightRange)
            return false;

        Exhausted?.Invoke(this, EventArgs.Empty);
        _dead = true;
        return true;
    }

    private bool HandleHit(HashSet<Enemy> enemies)
    {
        foreach (var enemy in enemies)
        {
            if (enemy.State is not EnemyState.Vulnerable)
                continue;

            var enemyCircle = enemy.HitCircle;
            var circle = new Circle(_position, _hitCircleRadius);

            if (enemyCircle.Intersects(circle))
            {
                var explosion = _explosionFactory.Create(
                    _explosionAlias,
                    _position,
                    _aoeRange,
                    _aoeDamage);
                Collided?.Invoke(this, explosion);


                enemy.ReceiveDamage(_directDamage);
                _dead = true;
                return true;
            }
        }

        return false;
    }

    private void HandleMove(float deltaSeconds)
    {
        var momentSpeed = _speed * deltaSeconds;

        _position += new Vector2(
            MathF.Cos(Rotation),
            MathF.Sin(Rotation)) * momentSpeed;

        _pathTraveled += momentSpeed;
    }
}