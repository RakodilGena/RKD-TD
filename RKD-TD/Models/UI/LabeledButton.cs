using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;

namespace RKD_TD.Models.UI;

internal sealed class LabeledButton : Button
{
    private readonly string _text;
    private readonly SpriteFont _textFont;
    private readonly Vector2 _textPosition, _textCenter;
    private readonly float _textScale;
    private readonly Color _textColor, _textHoverColor;

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
        float textScale,
        Color textColor,
        Color textHoverColor,
        float layerDepth)
        : base(position, origin, spriteIdle, spritePressed,
            scale, colorIdle, colorHover, layerDepth)
    {
        _text = text;
        _textFont = textFont;
        _textScale = textScale;
        _textColor = textColor;
        _textHoverColor = textHoverColor;

        _textCenter = _textFont.MeasureString(text) / 2;

        var buttonCenter = new Vector2(
            spriteIdle.Width * 0.5f,
            spriteIdle.Height * 0.5f);
        
        _textPosition = position + (-origin + buttonCenter) * scale;
    }


    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        spriteBatch.DrawString(
            _textFont,
            _text,
            _textPosition,
            Hovered ? _textHoverColor : _textColor,
            rotation: 0,
            origin: _textCenter,
            _textScale,
            SpriteEffects.None,
            LayerDepth);
    }
}