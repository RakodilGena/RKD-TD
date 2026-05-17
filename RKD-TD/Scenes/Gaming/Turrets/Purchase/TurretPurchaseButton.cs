using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics.Sprites;
using RKD_TD.Models.UI;

namespace RKD_TD.Scenes.Gaming.Turrets.Purchase;

internal sealed class TurretPurchaseButton : Button
{
    public TurretPurchaseButton(
        Vector2 position,
        Vector2 origin,
        Sprite spriteIdle,
        Sprite spriteHovered,
        Vector2 scale,
        float layerDepth)
        : base(position, origin, spriteIdle, spriteHovered, scale, layerDepth)
    {
    }
}