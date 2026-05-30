using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Geometrics;
using MonoGameLibrary.Visuals;
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

    public static Texture2D BuildOverlay = InitBuiltOverlay();

    private static Texture2D InitBuiltOverlay()
    {
        var texture = new Texture2D(Core.GraphicsDevice, 1, 1);
        texture.SetData([Color.White]);

        return texture;
    }

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

        var canPlace = IsBuildable && !IsOccupied;

        if (canPlace)
        {
            var cellCenter = WorldPosition + CellSize * 0.5f;

            Circle.DrawCircle(
                spriteBatch,
                camera,
                cellCenter,
                turret.Radius,
                Colors.Game.TurretRadius);

            turret.CarriageSprite.Draw(
                spriteBatch,
                cellCenter,
                camera);

            turret.BarrelSprite.Draw(
                spriteBatch,
                cellCenter,
                camera);
        }
        else
        {
            var (drawSize, screenPos) = camera.WorldToScreen(CellSize, WorldPosition);
            spriteBatch.Draw(
                BuildOverlay,
                new Rectangle((int)screenPos.X, (int)screenPos.Y, (int)drawSize.X, (int)drawSize.Y),
                color: Colors.Game.CellHighlight.NotAvailable);
        }
    }
}