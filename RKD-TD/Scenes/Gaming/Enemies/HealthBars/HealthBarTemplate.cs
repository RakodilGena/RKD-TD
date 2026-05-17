using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;

namespace RKD_TD.Scenes.Gaming.Enemies.HealthBars;

internal sealed record HealthBarTemplate(
    int Health,
    int HealthIncreasePerWave,
    float HealthMultiplierPerWave,
    Vector2 EnemyOffset,
    Vector2 Borders, //thats in offset for bg and filler
    TextureRegion BordersTexture,
    Vector2 BordersScale,
    Color BordersColor,
    TextureRegion BackgroundTexture,
    Color BackgroundColor,
    TextureRegion FillerTexture,
    Color FillerColor,
    Vector2 FillerScale //initial for both Background and Filler
);