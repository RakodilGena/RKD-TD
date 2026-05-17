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
        Sprite spriteIdle,
        Sprite spriteHovered,
        Sprite spritePressed,
        Vector2 scale,
        string text,
        SpriteFont textFont,
        Vector2 textScale,
        Color textColor,
        Color borderColor,
        Vector2 borderWidth,
        float layerDepth)
        : base(position, origin,
            spriteIdle, spriteHovered, spritePressed,
            scale, layerDepth)
    {
        var buttonCenter = new Vector2(
            spriteIdle.Width * 0.5f,
            spriteIdle.Height * 0.5f);

        var textPosition = position + (-origin + buttonCenter) * scale;

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