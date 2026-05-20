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

    private const int PADDING_Y = 5, PADDING_X = 15;

    public TurretPurchaseButton(
        string name,
        int price,
        SpriteFont nameFont,
        SpriteFont priceFont,
        Vector2 position,
        Vector2 origin,
        Sprite sprite,
        Color idleColor,
        Color hoveredColor,
        Vector2 scale,
        float layerDepth)
        : base(position, origin, sprite, idleColor, hoveredColor, scale, layerDepth)
    {
        _nameLabel = CreateNameLabel(name, nameFont, scale, out var textTopY);
        _priceLabel = CreatePriceLabel(price, priceFont, scale, textTopY);
    }

    private Label CreateNameLabel(string name, SpriteFont font, Vector2 scale, out float textTopY)
    {
        var scaledPaddingX = PADDING_X * scale.X;
        var scaledPaddingY = PADDING_Y * scale.Y;
        var buttonCenterX = Bounds.X + Bounds.Width * 0.5f;


        var maxNameLabelSizeX = Bounds.Width - scaledPaddingX * 2;
        var actualNameLabelSize = font.MeasureString(name);

        var nameLabelScaleX = MathF.Min(maxNameLabelSizeX / actualNameLabelSize.X, scale.X);

        var halfTextHeight = actualNameLabelSize.Y * scale.Y * 0.5f;
        var y = Bounds.Y + Bounds.Height - halfTextHeight - scaledPaddingY;

        var position = new Vector2(buttonCenterX, y);

        var nameLabel = new BorderedLabel(name, font)
        {
            Color = Color.White,
            BorderColor = Color.Black,

            BorderWidth = new Vector2(2),
            Position = position,
            Scale = new Vector2(nameLabelScaleX, scale.Y)
        };
        nameLabel.CenterOrigin();

        textTopY = y - halfTextHeight;

        return nameLabel;
    }

    private Label CreatePriceLabel(
        int price,
        SpriteFont font,
        Vector2 scale,
        float nameTextTopY)
    {
        var scaledPaddingX = PADDING_X * scale.X;

        var priceText = price.ToString();
        var textSize = font.MeasureString(priceText) * scale;

        var halfTextWidth = textSize.X * 0.5f;
        var x = Bounds.X + Bounds.Width - scaledPaddingX - halfTextWidth;

        var y = nameTextTopY - textSize.Y * 0.5f + 15 * scale.Y;

        var position = new Vector2(x, y);

        var priceLabel = new BorderedLabel(priceText, font)
        {
            Color = Color.Goldenrod,
            BorderColor = Color.Black,

            BorderWidth = new Vector2(2),
            Position = position,
            Scale = scale
        };
        priceLabel.CenterOrigin();

        return priceLabel;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        _nameLabel.Draw(spriteBatch);
        _priceLabel.Draw(spriteBatch);
    }
}