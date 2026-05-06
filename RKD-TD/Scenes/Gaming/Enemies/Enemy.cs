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
    private readonly Vector2 _positionInTile, _initialScale;

    private readonly Sprite _sprite;

    private int _currentWaypointIndex;
    private readonly WaypointPath _path;
    private Vector2 _position;
    private readonly Vector2 _origin;

    private readonly float _appearDistance;
    private float _pathTraveled;

    private bool
        _appeared,
        _finished;

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
        Vector2 positionInTile,
        Vector2 origin,
        float appearDistance)
    {
        _maxHealth = _currentHealth = maxHealth;
        _speed = speed;
        _reward = reward;
        // _textureScale = textureScale;
        // _texture = texture;
        // _animation = animation;
        _sprite = sprite;
        _initialScale = sprite.Scale;
        sprite.Scale = Vector2.Zero; //to correctly show enemy appearing from portal

        _path = path;
        _positionInTile = positionInTile;
        _origin = origin;
        _appearDistance = appearDistance;
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

    public void Update(float deltaSeconds)
    {
        if (_finished)
            return;

        _sprite.Update(deltaSeconds);

        if (HandleReachedEnd())
            return;

        if (HandleDestroyed())
            return;

        MoveTowardsPath(deltaSeconds);

        HandleAppear();

        HandleDisappear();
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

    private void MoveTowardsPath(float deltaSeconds)
    {
        Vector2 target = _path.Waypoints[_currentWaypointIndex];

        Vector2 direction = target - _position;
        float distanceToWaypoint = direction.Length();

        var distanceAtStep = _speed * deltaSeconds;

        if (distanceToWaypoint <= distanceAtStep)
        {
            // Snap to waypoint and advance
            _position = target;
            _currentWaypointIndex++;
            SetFaceDirection();

            _pathTraveled += distanceToWaypoint;
        }
        else
        {
            direction.Normalize();
            _position += direction * distanceAtStep;

            _pathTraveled += distanceAtStep;
        }
    }

    private void HandleAppear()
    {
        if (_appeared || _appearDistance is 0)
            return;

        if (_pathTraveled >= _appearDistance)
        {
            _appeared = true;
            _sprite.Scale = _initialScale;
            return;
        }

        var scale = _pathTraveled / _appearDistance;
        _sprite.Scale = _initialScale * scale;
    }

    private void HandleDisappear()
    {
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _sprite.Draw(spriteBatch, _position + _positionInTile + _origin);
    }
}