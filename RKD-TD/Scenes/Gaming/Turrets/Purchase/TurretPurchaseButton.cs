using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics.Labels;
using MonoGameLibrary.Graphics.Sprites;
using RKD_TD.Models.UI;

namespace RKD_TD.Scenes.Gaming.Turrets.Purchase;

internal sealed class TurretPurchaseButton : Button
{
    private readonly Label _nameLabel;
    private readonly Label _priceLabel;
        
    public TurretPurchaseButton(
        string name,
        int price,
        SpriteFont font,
        
        Vector2 position,
        Vector2 origin,
        Sprite spriteIdle,
        Sprite spriteHovered,
        Vector2 scale,
        float layerDepth)
        : base(position, origin, spriteIdle, spriteHovered, scale, layerDepth)
    {
        var buttonCenter = position + new Vector2(Bounds.Width * 0.5f, Bounds.Height * 0.5f);

        var maxNameLabelSize = Bounds.Width - 10;
        var actualNameLabelSize = font.MeasureString(name).X;
        var nameLabelScale = MathF.Min(maxNameLabelSize/actualNameLabelSize, 1f);
        
        var nameLabel = new BorderedLabel(name, font)
        {
            Color = Color.White,
            BorderWidth = new Vector2(2),
            BorderColor = Color.Black,
            Position = buttonCenter + new Vector2(0, 45),
            Scale = new Vector2(nameLabelScale, 1)
        };
        nameLabel.CenterOrigin();

        var priceText = price.ToString();
        var priceLabel = new BorderedLabel(priceText, font)
        {
            Color = Color.Goldenrod,
            BorderWidth = new Vector2(2),
            BorderColor = Color.Black,
            Position = buttonCenter + new Vector2(0, 15)
        };
        priceLabel.CenterOrigin();

        _nameLabel = nameLabel;
        _priceLabel = priceLabel;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        _nameLabel.Draw(spriteBatch);
        _priceLabel.Draw(spriteBatch);
    }
}