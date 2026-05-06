using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;
using RKD_TD.Models.UI;

namespace RKD_TD.Scenes.Gaming.PurchaseTurrets;

public sealed class TurretPurchaseButton : Button
{
    public TurretPurchaseButton(
        Vector2 position,
        Vector2 origin, 
        Sprite spriteIdle, 
        Sprite spriteHovered,
        Sprite spritePressed, 
        Vector2 scale, 
        float layerDepth) 
        : base(position, origin, spriteIdle, spriteHovered, spritePressed, scale, layerDepth)
    {
    }
}