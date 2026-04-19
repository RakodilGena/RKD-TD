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
        TextureRegion textureIdle,
        TextureRegion texturePressed,
        float scale,
        Color color,
        Color hoverColor,
        string text,
        SpriteFont textFont,
        float textScale,
        Color textColor,
        Color textHoverColor,
        float layerDepth)
        : base(position, origin, textureIdle, texturePressed,
            scale, color, hoverColor, layerDepth)
    {
        _text = text;
        _textFont = textFont;
        _textScale = textScale;
        _textColor = textColor;
        _textHoverColor = textHoverColor;

        _textCenter = _textFont.MeasureString(text) / 2;

        var buttonCenter = new Vector2(
            textureIdle.Width / 2f,
            textureIdle.Height / 2f);
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