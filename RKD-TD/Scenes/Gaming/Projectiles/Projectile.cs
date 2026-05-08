using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Cameras;
using MonoGameLibrary.Graphics.Sprites;

namespace RKD_TD.Scenes.Gaming.Projectiles;

internal class Projectile
{
    private readonly Sprite _sprite;

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
        _flightRange,
        _directDamage,
        _aoeDamage,
        _aoeRange;

    private Vector2 _position;

    private float _pathTraveled;

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

    public Projectile(
        Sprite sprite,
        float speed,
        float flightRange,
        float directDamage,
        float aoeDamage,
        float aoeRange,
        Vector2 position,
        float rotation)
    {
        _sprite = sprite;
        _speed = speed;
        _flightRange = flightRange;
        _directDamage = directDamage;
        _aoeDamage = aoeDamage;
        _aoeRange = aoeRange;
        Rotation = rotation;
        _position = position;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _sprite.Draw(spriteBatch, _position);
    }

    public void Update(float deltaSeconds)
    {
        if (_pathTraveled > _flightRange)
        {
            Exhausted?.Invoke(this, EventArgs.Empty);
            return;
        }

        var momentSpeed = _speed * deltaSeconds;

        _position += new Vector2(
            MathF.Cos(Rotation),
            MathF.Sin(Rotation)) * momentSpeed;

        _pathTraveled += momentSpeed;
    }
}