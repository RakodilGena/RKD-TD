using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Cameras;
using MonoGameLibrary.Graphics.Sprites;

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
    private readonly Vector2 _origin;


    private Vector2 _positionForMovement, _positionOnScreen;
    public Vector2 Target { get; private set; }

    private readonly float _appearDistance;
    private float _pathTraveled;

    public EnemyState State { get; private set; }

    public ICamera? Camera
    {
        get => _sprite.Camera;
        set
        {
            _sprite.Camera = value;
            RecalculateSecondaryPositions();
        }
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

        _positionForMovement = path.Start;
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
        if (State is EnemyState.Finished)
            return;

        _sprite.Update(deltaSeconds);

        if (HandleReachedEnd())
            return;

        if (HandleDestroyed())
            return;

        MoveTowardsPath(deltaSeconds);

        HandleState();

        RecalculateSecondaryPositions();
    }

    private bool HandleReachedEnd()
    {
        if (_currentWaypointIndex < _path.Waypoints.Length)
            return false;

        ReachedPortal?.Invoke(this, _damage);
        State = EnemyState.Finished;
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
        State = EnemyState.Finished;
        return true;
    }

    private void MoveTowardsPath(float deltaSeconds)
    {
        Vector2 target = _path.Waypoints[_currentWaypointIndex];

        Vector2 direction = target - _positionForMovement;
        float distanceToWaypoint = direction.Length();

        var distanceAtStep = _speed * deltaSeconds;

        if (distanceToWaypoint <= distanceAtStep)
        {
            // Snap to waypoint and advance
            _positionForMovement = target;
            _currentWaypointIndex++;
            SetFaceDirection();

            _pathTraveled += distanceToWaypoint;
        }
        else
        {
            direction.Normalize();
            _positionForMovement += direction * distanceAtStep;

            _pathTraveled += distanceAtStep;
        }
    }

    private void HandleState()
    {
        //reverse order so each frame only one state is handled or switched.
        HandleDisappear();

        HandleVulnerable();

        HandleAppear();
    }

    private void HandleAppear()
    {
        if (State is not EnemyState.Appearing || _appearDistance is 0)
            return;

        if (_pathTraveled >= _appearDistance)
        {
            State = EnemyState.Vulnerable;
            _sprite.Scale = _initialScale;
            return;
        }

        var scale = _pathTraveled / _appearDistance;
        _sprite.Scale = _initialScale * scale;
    }

    private void HandleVulnerable()
    {
        if (State is not EnemyState.Vulnerable)
            return;

        var distanceToFinish = _path.TotalDistance - _pathTraveled;
        if (distanceToFinish > _appearDistance)
            return;

        State = EnemyState.Disappearing;
    }

    private void HandleDisappear()
    {
        if (State is not EnemyState.Disappearing || _appearDistance is 0)
            return;

        var distanceToFinish = _path.TotalDistance - _pathTraveled;
        if (distanceToFinish < 0)
            return;

        Debug.Assert(distanceToFinish < _appearDistance);

        var scale = distanceToFinish / _appearDistance;
        _sprite.Scale = _initialScale * scale;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _sprite.Draw(spriteBatch, _positionOnScreen);
    }

    private void RecalculateSecondaryPositions()
    {
        CalculatePositionOnScreen();
        CalculateTarget();
    }

    private void CalculatePositionOnScreen()
    {
        var (originScale, _) = Camera.WorldToScreen(_initialScale, _origin);

        _positionOnScreen = _positionForMovement + _positionInTile + _origin * originScale;
    }

    private void CalculateTarget()
    {
        Target = _positionForMovement + _positionInTile + _origin;
        //todo: recalculate rectangle here?
    }

    public enum EnemyState
    {
        Appearing,
        Vulnerable,
        Disappearing,
        Finished
    }
}