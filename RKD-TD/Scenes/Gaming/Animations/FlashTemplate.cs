using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics.Sprites;

namespace RKD_TD.Scenes.Gaming.Animations;

internal sealed record FlashTemplate(
    string Alias,
    Animation Animation,
    Vector2 Origin,
    Vector2 Scale);