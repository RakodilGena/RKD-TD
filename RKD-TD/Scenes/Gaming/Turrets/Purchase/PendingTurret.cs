using MonoGameLibrary.Graphics.Sprites;
using RKD_TD.Assets;
using RKD_TD.Scenes.Gaming.Turrets.Active;

namespace RKD_TD.Scenes.Gaming.Turrets.Purchase;

internal sealed class PendingTurret
{
    public string Name { get; }
    public float Radius { get; }
    public TurretType Type { get; }

    public int Price { get; }

    public Sprite BarrelSprite { get; }
    public Sprite CarriageSprite { get; }

    public PendingTurret(
        TurretType type,
        TurretTemplate template)

    {
        Type = type;
        Name = template.Name;
        Radius = template.FiringDistance[0];
        Price = template.Price;

        var barrelTexture = template.BarrelTexture ?? template.BarrelAnimation!.Frames[0];

        BarrelSprite = new Sprite(barrelTexture)
        {
            Scale = template.BarrelScale,
            Origin = template.BarrelOrigin,
            Color = Colors.Game.CellHighlight.Available
        };

        CarriageSprite = new Sprite(template.CarriageTexture)
        {
            Scale = template.CarriageScale,
            Origin = template.CarriageOrigin,
            Color = Colors.Game.CellHighlight.Available
        };
    }
}