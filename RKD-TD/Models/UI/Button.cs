using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;

namespace RKD_TD.Models.UI;

internal sealed class Button : IMyDrawable
{
    private readonly Vector2 _position;

    private readonly TextureRegion _textureIdle, _texturePressed;
    private readonly Vector2 _center;
    private readonly float _scale;
    private readonly Color _color, _hoverColor;

    private readonly float _layerDepth;

    private readonly string _text;
    private readonly SpriteFont _textFont;
    private readonly Vector2 _textCenter;
    private readonly float _textScale;
    private readonly Color _textColor, _textHoverColor;

    private Rectangle _body;
    private bool _buttonPressed, _hovered;

    public event EventHandler? Pressed;

    public Button(
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
    {
        _position = position;
        _textureIdle = textureIdle;
        _texturePressed = texturePressed;

        _center = new Vector2(
            textureIdle.Width / 2,
            textureIdle.Height / 2);

        _scale = scale;
        _color = color;
        _hoverColor = hoverColor;
        _text = text;
        _textFont = textFont;
        _textScale = textScale;
        _textColor = textColor;
        _textHoverColor = textHoverColor;

        _body = new Rectangle(
            (int)(position.X - textureIdle.Width / 2 * scale),
            (int)(position.Y - textureIdle.Height / 2 * scale),
            (int)(textureIdle.Width * scale),
            (int)(textureIdle.Height * scale));

        _textCenter = _textFont.MeasureString(text) / 2;

        _layerDepth = layerDepth;
    }

    public void Update(GameTime gameTime)
    {
        var mouseInfo = Core.Input.Mouse;
        if (_body.Contains(mouseInfo.Position))
        {
            _hovered = true;

            //added control over prev lmb mouse state to prevent cases when button 
            //wasn't pressed but the lmb was - outside the button - and that still
            //invoked the click event
            if (!_buttonPressed)
            {
                if (mouseInfo.WasButtonJustPressed(MouseButton.Left))
                    _buttonPressed = true;
            }
            else
            {
                if (mouseInfo.WasButtonJustReleased(MouseButton.Left))
                {
                    _buttonPressed = false;
                    Pressed?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        else
        {
            _hovered = false;
            _buttonPressed = false;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var (texture, srcRect) = _buttonPressed
            ? (_texturePressed.Texture, _texturePressed.SourceRectangle)
            : (_textureIdle.Texture, _textureIdle.SourceRectangle);

        spriteBatch.Draw(
            texture,
            _position,
            srcRect,
            _hovered ? _hoverColor : _color,
            0f,
            origin: _center,
            _scale,
            SpriteEffects.None,
            layerDepth: _layerDepth);

        spriteBatch.DrawString(
            _textFont,
            _text,
            _position,
            _hovered ? _textHoverColor : _textColor,
            0,
            _textCenter,
            _textScale,
            SpriteEffects.None,
            _layerDepth);
    }
}