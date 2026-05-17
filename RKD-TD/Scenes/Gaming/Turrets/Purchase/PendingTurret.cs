using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using RKD_TD.Scenes.Gaming.Turrets.Active;

namespace RKD_TD.Scenes.Gaming.Turrets.Purchase;

internal sealed class PendingTurret
{
    public float Radius { get; }
    public TurretType Type { get; }
    public Texture2D Texture { get; }

    public int Price { get; }

    public PendingTurret(
        TurretType type,
        int price,
        float radius)

    {
        Radius = radius;
        Type = type;
        Price = price;

        Texture = new Texture2D(Core.GraphicsDevice, 1, 1);
        Texture.SetData([Color.White]);
    }
}