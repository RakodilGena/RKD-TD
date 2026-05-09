using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Graphics.Sprites;

namespace RKD_TD.Scenes.Gaming.ActiveTurrets;

internal sealed record TurretTemplate(
    TextureRegion? BarrelTexture,
    Animation? BarrelAnimation,
    Vector2 BarrelScale,
    Vector2 BarrelOrigin,
    TextureRegion CarriageTexture,
    Vector2 CarriageScale,
    Vector2 CarriageOrigin,
    float RotationSpeedRadianInSec,
    float ReloadTimeInSec,
    float FixateDistanceSquared,
    float FiringDistanceSquared,
    TurretFiringPoint[] FiringPoints,
    TurretFiringMode FiringMode, 
    Vector2[] GunFlashPoints,
    string ProjectileAlias,
    string FlashAlias);