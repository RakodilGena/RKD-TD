using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Cameras;
using MonoGameLibrary.Graphics;

namespace RKD_TD.Scenes.Gaming.Enemies;

internal class Enemy
{
    private readonly int _maxHealth;
    private int _currentHealth;
    private readonly float _speed;
    private readonly int _reward;
    private readonly int _damage;
    private readonly Vector2 _positionInTile;

    private readonly Sprite _sprite;

    private int _currentWaypointIndex;
    private readonly WaypointPath _path;
    private Vector2 _position;
    private bool _finished;

    public ICamera? Camera
    {
        get => _sprite.Camera;
        set => _sprite.Camera = value;
    }

    //todo: subscribe and shit.
    public event EventHandler<int>? Destroyed;
    public event EventHandler<int>? ReachedPortal;

    public Enemy(
        int maxHealth,
        float speed,
        int reward,
        int damage,
        Sprite sprite,
        WaypointPath path,
        Vector2 positionInTile)
    {
        _maxHealth = _currentHealth = maxHealth;
        _speed = speed;
        _reward = reward;
        // _textureScale = textureScale;
        // _texture = texture;
        // _animation = animation;
        _sprite = sprite;

        _path = path;
        _positionInTile = positionInTile;
        _damage = damage;

        _position = path.Start;
        _currentWaypointIndex = 1; // start moving toward waypoint 1
        SetFaceDirection();
    }

    private void SetFaceDirection()
    {
        if (_currentWaypointIndex >= _path.Waypoints.Length)
            return;

        var prev = _path.Waypoints[_currentWaypointIndex - 1];
        var next = _path.Waypoints[_currentWaypointIndex];

        var vector = next - prev;
        switch (vector.X)
        {
            case < 0:
                _sprite.Effects = SpriteEffects.FlipHorizontally;
                break;

            case > 0:
                _sprite.Effects = SpriteEffects.None;
                break;
        }
    }

    public void Update(GameTime gameTime)
    {
        if (_finished)
            return;

        _sprite.Update(gameTime);

        if (HandleReachedEnd())
            return;

        if (HandleDestroyed())
            return;

        MoveTowardsPath(gameTime);
    }

    private bool HandleReachedEnd()
    {
        if (_currentWaypointIndex < _path.Waypoints.Length)
            return false;

        ReachedPortal?.Invoke(this, _damage);
        _finished = true;
        return true;
    }

    private void ReceiveDamage(int damage)
    {
        _currentHealth -= damage;
    }

    private bool HandleDestroyed()
    {
        if (_currentHealth > 0)
            return false;

        Destroyed?.Invoke(this, _reward);
        _finished = true;
        return true;
    }

    private void MoveTowardsPath(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Vector2 target = _path.Waypoints[_currentWaypointIndex];

        Vector2 direction = target - _position;
        float distanceToWaypoint = direction.Length();

        var distanceAtStep = _speed * delta;

        if (distanceToWaypoint <= distanceAtStep)
        {
            // Snap to waypoint and advance
            _position = target;
            _currentWaypointIndex++;
            SetFaceDirection();
        }
        else
        {
            direction.Normalize();
            _position += direction * distanceAtStep;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _sprite.Draw(spriteBatch, _position + _positionInTile);
    }
}