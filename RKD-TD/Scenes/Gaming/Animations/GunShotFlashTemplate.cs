using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics.Sprites;

namespace RKD_TD.Scenes.Gaming.Animations;

internal sealed record GunShotFlashTemplate(
    string Alias,
    Animation Animation,
    Vector2 Origin,
    Vector2 Scale);