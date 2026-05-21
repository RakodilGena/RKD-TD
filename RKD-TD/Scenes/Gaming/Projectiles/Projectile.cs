using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Cameras;
using MonoGameLibrary.Extensions;
using MonoGameLibrary.Geometrics;
using MonoGameLibrary.Graphics.Sprites;
using RKD_TD.Scenes.Gaming.Enemies;
using RKD_TD.Scenes.Gaming.Explosions;
using RKD_TD.Scenes.Gaming.Flashes;
using RKD_TD.Scenes.Gaming.Turrets.Active;

namespace RKD_TD.Scenes.Gaming.Projectiles;

internal class Projectile
{
    private readonly Turret _owner;
    
    private readonly Sprite _sprite;

    private readonly string _explosionAlias;
    private readonly ExplosionFactory _explosionFactory;

    private readonly string? _trailFlashAlias;
    private readonly FlashFactory _flashFactory;
    private readonly Vector2 _trailFlashSpawnOffset;
    private float _trailFlashSpawnTimer;

    protected float Rotation
    {
        get;
        set
        {
            field = value;
            _sprite.Rotation = value;
        }
    }

    private readonly float _speed;

    private readonly float
        _flightRange,
        _trailFlashSpawnPauseSec;

    private readonly int _directDamage, _aoeRange, _aoeDamage, _hitCircleRadius;

    protected Vector2 Position { get; private set; }

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

    public event EventHandler<Flash>? TrailFlashEmitted;

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
        string? trailFlashAlias,
        float trailFlashSpawnPauseSec,
        Vector2 trailFlashSpawnOffset,
        ExplosionFactory explosionFactory,
        FlashFactory flashFactory, 
        
        Turret owner)
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
        _flashFactory = flashFactory;
        _owner = owner;
        _trailFlashSpawnOffset = trailFlashSpawnOffset;
        _trailFlashAlias = trailFlashAlias;

        _trailFlashSpawnTimer = trailFlashSpawnPauseSec;
        _trailFlashSpawnPauseSec = trailFlashSpawnPauseSec;

        _hitCircleRadius = hitCircleRadius;
        Position = position;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _sprite.Draw(spriteBatch, Position);

        if (GameCore.DRAW_HIT_BOX)
#pragma warning disable CS0162 // Unreachable code detected
        {
            Circle.DrawCircle(spriteBatch, Camera, Position, _hitCircleRadius, Color.BlueViolet);
        }
#pragma warning restore CS0162 // Unreachable code detected
    }

    public void Update(
        HashSet<Enemy> enemies,
        float deltaSeconds)
    {
        if (_dead)
            return;

        _sprite.Update(deltaSeconds);

        if (HandleExhausted())
            return;

        if (HandleHit(enemies))
            return;

        HandleMove(deltaSeconds);

        HandleTrailFlashing(deltaSeconds);
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
            var circle = new Circle(Position, _hitCircleRadius);

            if (!enemyCircle.Intersects(circle))
                continue;

            Explode(enemy);

            return true;
        }

        return false;
    }

    private void Explode(Enemy enemy)
    {
        var explosion = _explosionFactory.Create(
            _explosionAlias,
            Position,
            _aoeRange,
            _aoeDamage,
            _owner);
        
        Collided?.Invoke(this, explosion);

        var damageDealt = enemy.ReceiveDamage(_directDamage);
        _owner.RecordDealtDamage(damageDealt);

        _dead = true;
    }

    protected virtual void HandleMove(float deltaSeconds)
    {
        var momentSpeed = _speed * deltaSeconds;

        Position += Rotation.ToUnitVector2() * momentSpeed;

        _pathTraveled += momentSpeed;
    }

    private void HandleTrailFlashing(float deltaSeconds)
    {
        if (_trailFlashAlias is null)
            return;

        if (_trailFlashSpawnTimer > _trailFlashSpawnPauseSec)
        {
            _trailFlashSpawnTimer -= _trailFlashSpawnPauseSec;

            var offset = _trailFlashSpawnOffset.GetRotatedVector(Rotation);

            var position = Position + offset;

            var angle = MathHelper.ToRadians(Random.Shared.Next(0, 360));

            var flash = _flashFactory.Create(
                _trailFlashAlias,
                position,
                angle);

            TrailFlashEmitted?.Invoke(this, flash);
            return;
        }

        _trailFlashSpawnTimer += deltaSeconds;
    }
}