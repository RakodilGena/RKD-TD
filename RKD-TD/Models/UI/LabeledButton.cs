using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;

namespace RKD_TD.Models.UI;

internal sealed class LabeledButton : Button
{
    private readonly string _text;
    private readonly SpriteFont _textFont;
    private readonly Vector2 _textCenter;
    private readonly float _textScale;
    private readonly Color _textColor, _textHoverColor;

    public LabeledButton(
        Vector2 position,
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
        : base(position, textureIdle, texturePressed,
            scale, color, hoverColor, layerDepth)
    {
        _text = text;
        _textFont = textFont;
        _textScale = textScale;
        _textColor = textColor;
        _textHoverColor = textHoverColor;
        _textCenter = _textFont.MeasureString(text) / 2;
    }


    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        spriteBatch.DrawString(
            _textFont,
            _text,
            Position,
            Hovered ? _textHoverColor : _textColor,
            0,
            _textCenter,
            _textScale,
            SpriteEffects.None,
            LayerDepth);
    }
}