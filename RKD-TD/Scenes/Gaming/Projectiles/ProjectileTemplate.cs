using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Graphics.Sprites;

namespace RKD_TD.Scenes.Gaming.Projectiles;

internal sealed record ProjectileTemplate(
    string Alias,
    TextureRegion? Texture,
    Animation? Animation,
    Vector2 Origin,
    Vector2 Scale,
    float Speed,
    float FlightRange,
    float DirectDamage,
    float AoeRange,
    float AoeDamage);