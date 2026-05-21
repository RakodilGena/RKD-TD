using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Cameras;
using MonoGameLibrary.Geometrics;
using RKD_TD.Assets;
using RKD_TD.Scenes.Gaming.Turrets.Active;

namespace RKD_TD.Scenes.Gaming.Turrets.Purchase;

internal sealed class BuildCell
{
    public Vector2 WorldPosition { get; init; } // top-left corner in world space

    public Vector2 CellSize { get; init; }

    public Turret? BuiltTurret { get; private set; }

    public bool IsOccupied => BuiltTurret is not null;
    public bool IsBuildable { get; init; } = true;

    public void Occupy(Turret turret)
    {
        BuiltTurret = turret;
    }

    public void Deoccupy()
    {
        BuiltTurret = null;
    }

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
            Circle.DrawCircle(
                spriteBatch,
                camera,
                circlePosition,
                turret.Radius,
                Colors.Game.TurretRadius);
        }

        // green = valid, red = blocked
        Color highlight = canPlace
            ? Colors.Game.CellHighlight.Available
            : Colors.Game.CellHighlight.NotAvailable;

        spriteBatch.Draw(
            turret.Texture,
            new Rectangle((int)screenPos.X, (int)screenPos.Y, (int)drawSize.X, (int)drawSize.Y),
            highlight);
    }
}