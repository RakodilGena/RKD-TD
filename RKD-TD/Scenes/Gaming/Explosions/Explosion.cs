using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Geometrics;
using MonoGameLibrary.Graphics.Sprites;
using RKD_TD.Scenes.Gaming.Enemies;
using RKD_TD.Scenes.Gaming.Flashes;
using RKD_TD.Scenes.Gaming.Turrets.Active;

namespace RKD_TD.Scenes.Gaming.Explosions;

internal sealed class Explosion : Flash
{
    private readonly Turret _owner;

    private readonly Circle _damageCircle;
    private readonly float _damageDelaySec;
    private readonly int _aoeDamage;
    private readonly bool _explosionCanHappen;
    private bool _explosionHappened;

    public Explosion(
        AnimatedSprite sprite,
        Vector2 position,
        int aoeRange,
        int aoeDamage,
        float damageDelaySec,
        Turret owner) : base(sprite, position)
    {
        _aoeDamage = aoeDamage;
        _damageDelaySec = damageDelaySec;
        _owner = owner;
        _damageCircle = new Circle(position, aoeRange);

        _explosionCanHappen =
            aoeDamage > 0
            && aoeRange > 0
            && damageDelaySec >= 0;
    }

    public void Update(float deltaSeconds, HashSet<Enemy> enemies)
    {
        HandleDamage(enemies);

        Update(deltaSeconds);
    }

    private void HandleDamage(HashSet<Enemy> enemies)
    {
        //if no explosion can happen OR  explosion has happened or it's not yet time
        if (!_explosionCanHappen
            || _explosionHappened
            || ElapsedTimeSec < _damageDelaySec)
            return;

        _explosionHappened = true;

        //no one to damage
        if (enemies.Count is 0)
            return;

        foreach (var enemy in enemies)
        {
            if (enemy.State is not EnemyState.Vulnerable)
                continue;

            var enemyCircle = enemy.HitCircle;

            if (enemyCircle.Intersects(_damageCircle))
            {
                var damageDealt = enemy.ReceiveDamage(_aoeDamage);
                _owner.RecordDealtDamage(damageDealt);
            }
        }
    }

    [SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

#pragma warning disable CS0162 // Unreachable code detected
        if (GameCore.DRAW_HIT_BOX && _explosionCanHappen)
        {
            Circle.DrawCircle(spriteBatch, Camera, Position, _damageCircle.Radius, Color.White);
        }
#pragma warning restore CS0162 // Unreachable code detected
    }
}