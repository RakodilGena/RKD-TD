using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics.Sprites;

namespace RKD_TD.Scenes.Gaming.Flashes;

internal sealed record FlashTemplate(
    string Alias,
    Animation Animation,
    Vector2 Origin,
    Vector2 Scale);