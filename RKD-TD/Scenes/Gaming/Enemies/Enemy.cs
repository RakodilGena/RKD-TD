using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Geometrics;
using MonoGameLibrary.Graphics.Sprites;
using MonoGameLibrary.Visuals;
using RKD_TD.Assets;
using RKD_TD.Scenes.Gaming.Enemies.HealthBars;
using RKD_TD.Scenes.Gaming.Enemies.Spawns;

namespace RKD_TD.Scenes.Gaming.Enemies;

internal class Enemy
{
    private readonly HealthBar _healthBar;
    public float Speed { get; }
    private readonly int _reward;
    private readonly int _damage;
    private readonly Vector2 _positionInTile, _initialScale;

    private readonly Sprite _sprite;

    private int _currentWaypointIndex;
    private readonly WaypointPath _path;
    private readonly Vector2 _origin;

    private readonly int _hitCircleRadius;
    private readonly Vector2 _hitCircleOffset;
    private bool _facesRight = true;

    private Vector2 _positionForMovement;

    private Vector2 _positionOnScreen;
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
            _healthBar.Camera = value;
            CalculateSecondaryPositions();
        }
    }

    private readonly float _dyingTimeSec = 1f;
    private float _dyingTimer, _dyingAngle;
    private Vector2 _dyingShrink;


    public event EventHandler<int>? Destroyed;
    public event EventHandler<int>? ReachedPortal;

    public Enemy(
        EnemyTemplate template,
        Sprite sprite,
        WaypointPath path,
        Vector2 positionInTile,
        int waveIndex)
    {
        Speed = template.Speed;
        _reward = template.Reward;

        _sprite = sprite;
        _initialScale = sprite.Scale;
        sprite.Scale = Vector2.Zero; //to correctly show enemy appearing from portal

        _path = path;
        _positionInTile = positionInTile;
        _origin = template.Origin;
        _appearDistance = template.AppearDistance;
        _hitCircleRadius = template.HitCircleRadius;
        _hitCircleOffset = template.HitCircleOffset;
        _damage = template.Damage;

        _positionForMovement = path.Start;
        _currentWaypointIndex = 1; // start moving toward waypoint 1

        SetFaceDirection();

        _healthBar = new HealthBar(
            template.HealthBarTemplate,
            waveIndex);
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
                _facesRight = false;
                break;

            case > 0:
                _sprite.Effects = SpriteEffects.None;
                _facesRight = true;
                break;
        }
    }

    public void Update(float deltaSeconds)
    {
        if (State is EnemyState.Finished)
            return;

        if (HandleDying(deltaSeconds))
            return;

        _sprite.Update(deltaSeconds);

        if (HandleReachedEnd())
            return;

        if (HandleDestroyed())
            return;

        MoveTowardsPath(deltaSeconds);

        HandleState();

        CalculateSecondaryPositions();
    }

    private bool HandleReachedEnd()
    {
        if (_currentWaypointIndex < _path.Waypoints.Length)
            return false;

        ReachedPortal?.Invoke(this, _damage);
        State = EnemyState.Finished;
        return true;
    }

    public int ReceiveDamage(int damage) => _healthBar.ReceiveDamage(damage);

    private bool HandleDestroyed()
    {
        if (_healthBar.CurrentHealth > 0)
            return false;

        State = EnemyState.Dying;

        _sprite.Color = Colors.Game.EnemyDying;
        _dyingAngle = MathF.PI / _dyingTimeSec;
        _dyingShrink = _sprite.Scale / _dyingTimeSec;
        return true;
    }

    private bool HandleDying(float deltaSeconds)
    {
        if (State is not EnemyState.Dying)
            return false;

        //handle dying finished
        if (_dyingTimer > _dyingTimeSec)
        {
            State = EnemyState.Finished;
            Destroyed?.Invoke(this, _reward);
            return true;
        }

        _dyingTimer += deltaSeconds;

        var angleDelta = _dyingAngle * deltaSeconds;
        if (_facesRight)
        {
            _sprite.Rotation -= angleDelta;
        }
        else
        {
            _sprite.Rotation += angleDelta;
        }

        var shrinkDelta = _dyingShrink * deltaSeconds;
        _sprite.Scale -= shrinkDelta;

        return true;
    }

    private void MoveTowardsPath(float deltaSeconds)
    {
        Vector2 target = _path.Waypoints[_currentWaypointIndex];

        Vector2 direction = target - _positionForMovement;
        float distanceToWaypoint = direction.Length();

        var distanceAtStep = Speed * deltaSeconds;

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
        _healthBar.Draw(spriteBatch, State);

        if (GameCore.DRAW_HIT_BOX)
#pragma warning disable CS0162 // Unreachable code detected
        {
            Circle.DrawCircle(spriteBatch, Camera, Target, _hitCircleRadius, Color.Red);
        }
#pragma warning restore CS0162 // Unreachable code detected
    }

    private void CalculateSecondaryPositions()
    {
        _positionOnScreen = _positionForMovement + _positionInTile + _origin * _initialScale;
        _healthBar.SetPosition(_positionOnScreen);

        if (_hitCircleOffset == Vector2.Zero)
        {
            Target = _positionOnScreen;
            return;
        }

        if (_facesRight)
        {
            Target = _positionOnScreen + _hitCircleOffset;
        }
        else
        {
            Target = _positionOnScreen + new Vector2(-_hitCircleOffset.X, _hitCircleOffset.Y);
        }
    }

    public Vector2 GetDirectionUnitVector()
    {
        Vector2 target = _path.Waypoints[_currentWaypointIndex];
        if (target == _positionForMovement)
        {
            //overflow control
            if (_currentWaypointIndex >= _path.Waypoints.Length - 1)
                return Vector2.Zero;

            //this is here because sometimes float inaccuracies can lead to target being the same as positon,
            //but _currentWaypointIndex will change only the next frame
            target = _path.Waypoints[_currentWaypointIndex + 1];
        }

        Vector2 direction = target - _positionForMovement;
        direction.Normalize();
        return direction;
    }

    public Circle HitCircle => new(Target, _hitCircleRadius);
}