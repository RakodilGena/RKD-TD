using System;
using RKD_TD.Scenes.Gaming.Animations;
using RKD_TD.Scenes.Gaming.Projectiles;

namespace RKD_TD.Scenes.Gaming.ActiveTurrets;

internal readonly ref struct TurretShotEventArgs
{
    public readonly ReadOnlySpan<Projectile> Projectiles;
    public readonly ReadOnlySpan<GunShotFlash> Flashes;

    public TurretShotEventArgs(
        ReadOnlySpan<Projectile> projectiles,
        ReadOnlySpan<GunShotFlash> flashes)
    {
        Projectiles = projectiles;
        Flashes = flashes;
    }
}