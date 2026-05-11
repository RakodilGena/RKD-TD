using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Cameras;
using MonoGameLibrary.Collisions;

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
        if (turret is null)
            return;

        var (drawSize, screenPos) = camera.WorldToScreen(CellSize, WorldPosition);

        var canPlace = IsBuildable && !IsOccupied;

        if (canPlace)
        {
            var circlePosition = WorldPosition + CellSize * 0.5f;
            Circle.DrawHitCircle(
                spriteBatch,
                camera,
                circlePosition,
                turret.Radius,
                new Color(0, 0, 0, alpha: 80));
            //new Color(255, 206, 8, alpha: 240));
        }

        // green = valid, red = blocked
        Color highlight = canPlace
            ? new Color(0, 255, 0, 60)
            : new Color(255, 0, 0, 60);

        spriteBatch.Draw(
            turret.Texture,
            new Rectangle((int)screenPos.X, (int)screenPos.Y, (int)drawSize.X, (int)drawSize.Y),
            highlight);
    }
}