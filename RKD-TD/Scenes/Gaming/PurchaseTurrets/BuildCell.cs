using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Cameras;

namespace RKD_TD.Scenes.Gaming.PurchaseTurrets;

internal sealed class BuildCell
{
    public Vector2 WorldPosition { get; init; } // top-left corner in world space

    public Vector2 CellSize { get; init; }
    public bool IsOccupied { get; set; } = false;
    public bool IsBuildable { get; init; } = true;

    public void DrawPlacementOverlay(
        SpriteBatch spriteBatch,
        PendingTurret? turret,
        ICamera camera)
    {
        if (turret is null) return;

        var (drawSize, screenPos) = camera.WorldToScreen(CellSize, WorldPosition);

        // green = valid, red = blocked
        Color highlight = IsBuildable && !IsOccupied
            ? new Color(0, 255, 0, 80)
            : new Color(255, 0, 0, 80);

        spriteBatch.Draw(
            turret.Texture,
            new Rectangle((int)screenPos.X, (int)screenPos.Y, (int)drawSize.X, (int)drawSize.Y),
            highlight);
    }
}