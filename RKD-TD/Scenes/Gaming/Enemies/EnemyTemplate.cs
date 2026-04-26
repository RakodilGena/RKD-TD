using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;

namespace RKD_TD.Scenes.Gaming.Enemies;

internal sealed record EnemyTemplate(
    string Alias,
    int Health,
    float Speed,
    int Cost,
    int Reward,
    int Damage,
    Vector2 TextureScale,
    TextureRegion? Texture,
    Animation? Animation);