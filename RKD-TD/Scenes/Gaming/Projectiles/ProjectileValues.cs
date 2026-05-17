namespace RKD_TD.Scenes.Gaming.Projectiles;

internal readonly record struct ProjectileValues(
    float FlightRange,
    int DirectDamage,
    int AoeRange,
    int AoeDamage);