using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Graphics.Sprites;
using RKD_TD.Scenes.Gaming.Enemies.HealthBars;

namespace RKD_TD.Scenes.Gaming.Enemies.Spawns;

internal sealed record EnemyTemplate(
    string Alias,
    bool IsBoss,
    float Speed,
    int Cost,
    int Reward,
    int Damage,
    float AppearDistance,
    EnemyType Type,
    Vector2 TextureScale,
    TextureRegion? Texture,
    Animation? Animation,
    Vector2 Origin,
    int HitCircleRadius,
    Vector2 HitCircleOffset,
    HealthBarTemplate HealthBarTemplate);

public enum EnemyType
{
    Regular = 0,
    Swarm = 1
}