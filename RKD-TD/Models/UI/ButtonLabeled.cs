using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics.Labels;
using MonoGameLibrary.Graphics.Sprites;

namespace RKD_TD.Models.UI;

internal sealed class ButtonLabeled : Button
{
    private readonly Label _btnLabel;

    public ButtonLabeled(
        Vector2 position,
        Vector2 origin,
        Sprite sprite,
        Color idleColor,
        Color hoveredColor,
        Vector2 scale,
        string text,
        SpriteFont textFont,
        Vector2 textScale,
        Color textColor,
        Color borderColor,
        Vector2 borderWidth,
        float layerDepth)
        : base(position, origin, sprite,
            idleColor, hoveredColor, scale, layerDepth)
    {
        var buttonCenter = new Vector2(
            sprite.Width * 0.5f,
            sprite.Height * 0.5f);

        var textPosition = position + (-origin * scale + buttonCenter);

        _btnLabel = new BorderedLabel(
            textPosition,
            origin: Vector2.Zero,
            text,
            textFont,
            textColor,
            textScale,
            layerDepth,
            borderWidth,
            borderColor);

        _btnLabel.CenterOrigin();
    }


    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        _btnLabel.Draw(spriteBatch);
    }
}