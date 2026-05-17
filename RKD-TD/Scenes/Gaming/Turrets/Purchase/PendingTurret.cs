using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using RKD_TD.Scenes.Gaming.Turrets.Active;

namespace RKD_TD.Scenes.Gaming.Turrets.Purchase;

internal sealed class PendingTurret
{
    public string Name { get; }
    public float Radius { get; }
    public TurretType Type { get; }
    public Texture2D Texture { get; }

    public int Price { get; }

    public PendingTurret(
        string name,
        TurretType type,
        int price,
        float radius)

    {
        Name = name;
        Radius = radius;
        Type = type;
        Price = price;

        Texture = new Texture2D(Core.GraphicsDevice, 1, 1);
        Texture.SetData([Color.White]);
    }
}