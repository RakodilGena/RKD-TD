using Microsoft.Xna.Framework;

namespace RKD_TD.Scenes.Gaming.Turrets.Active;

internal readonly record struct TurretFiringPoint(
    Vector2 Position,
    float BulletExtraAngleInRadians);