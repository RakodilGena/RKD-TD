using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Cameras;
using RKD_TD.Scenes.Gaming.ActiveTurrets;

namespace RKD_TD.Scenes.Gaming.PurchaseTurrets;

internal sealed class PendingTurret
{
    public TurretType Type { get; }
    public Texture2D Texture { get; }

    public int Price { get; }

    public ICamera Camera { get; } //todo remove if not needed.

    public PendingTurret(
        TurretType type,
        ICamera camera,
        int price)
    {
        Type = type;

        Texture = new Texture2D(Core.GraphicsDevice, 1, 1);
        Texture.SetData([Color.White]);

        Camera = camera;
        Price = price;
    }
}