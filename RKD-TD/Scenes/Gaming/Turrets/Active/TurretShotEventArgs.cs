using System;
using RKD_TD.Scenes.Gaming.Flashes;
using RKD_TD.Scenes.Gaming.Projectiles;

namespace RKD_TD.Scenes.Gaming.Turrets.Active;

internal readonly ref struct TurretShotEventArgs
{
    public readonly ReadOnlySpan<Projectile> Projectiles;
    public readonly ReadOnlySpan<Flash> Flashes;

    public TurretShotEventArgs(
        ReadOnlySpan<Projectile> projectiles,
        ReadOnlySpan<Flash> flashes)
    {
        Projectiles = projectiles;
        Flashes = flashes;
    }
}