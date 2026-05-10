using Microsoft.Xna.Framework;
using MonoGameLibrary.Extensions;
using MonoGameLibrary.Graphics.Sprites;
using RKD_TD.Scenes.Gaming.Enemies;
using RKD_TD.Scenes.Gaming.Explosions;
using RKD_TD.Scenes.Gaming.Flashes;

namespace RKD_TD.Scenes.Gaming.Projectiles;

internal sealed class HomingMissile : Projectile
{
    private Enemy? _targetedEnemy;

    public HomingMissile(
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
        FlashFactory flashFactory)
        : base(sprite, speed, flightRange, hitCircleRadius, directDamage, aoeDamage, aoeRange,
            position, rotation, explosionAlias, trailFlashAlias, trailFlashSpawnPauseSec,
            trailFlashSpawnOffset, explosionFactory, flashFactory)
    {
    }

    public void SetTarget(Enemy enemy)
    {
        _targetedEnemy = enemy;
    }

    protected override void HandleMove(float deltaSeconds)
    {
        if (_targetedEnemy is not null)
        {
            if (_targetedEnemy.State is not EnemyState.Vulnerable)
            {
                _targetedEnemy = null;
            }
            else
            {
                var vectorToEnemy = _targetedEnemy.Target - Position;
                var desiredAngle = vectorToEnemy.ToAngle();
                Rotation = desiredAngle;
            }
        }

        base.HandleMove(deltaSeconds);
    }
}