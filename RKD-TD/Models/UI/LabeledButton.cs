using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;

namespace RKD_TD.Models.UI;

internal sealed class LabeledButton : Button
{
    private readonly Label _btnLabel;

    public LabeledButton(
        Vector2 position,
        Vector2 origin,
        Sprite spriteIdle,
        Sprite spritePressed,
        Vector2 scale,
        Color colorIdle,
        Color colorHover,
        string text,
        SpriteFont textFont,
        Vector2 textScale,
        Color textColor,
        float layerDepth)
        : base(position, origin, spriteIdle, spritePressed,
            scale, colorIdle, colorHover, layerDepth)
    {
        var buttonCenter = new Vector2(
            spriteIdle.Width * 0.5f,
            spriteIdle.Height * 0.5f);

        var textCenter = textFont.MeasureString(text) / 2;

        var textPosition = position + (-origin + buttonCenter) * scale;

        _btnLabel = new Label(
            textPosition,
            origin: textCenter,
            text,
            textFont,
            textColor,
            textScale,
            layerDepth);
    }


    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        _btnLabel.Draw(spriteBatch);
    }
}