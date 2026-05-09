using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics.Sprites;

namespace RKD_TD.Scenes.Gaming.Explosions;

internal sealed record ExplosionTemplate(
    string Alias,
    Animation Animation,
    Vector2 Origin,
    Vector2 Scale,
    float DamageDelaySec);