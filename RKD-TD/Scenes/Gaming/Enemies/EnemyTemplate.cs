using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Graphics.Sprites;

namespace RKD_TD.Scenes.Gaming.Enemies;

internal sealed record EnemyTemplate(
    string Alias,
    int Health,
    float Speed,
    int Cost,
    int Reward,
    int Damage,
    float AppearDistance,
    EnemyType Type,
    Vector2 TextureScale,
    TextureRegion? Texture,
    Animation? Animation,
    Vector2 Origin);

public enum EnemyType
{
    Regular = 0,
    Swarm = 1
}