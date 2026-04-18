using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RKD_TD.Models.UI;

internal sealed class Button : IMyDrawable
{
    private Vector2 _position;

    private Texture2D _textureIdle;
    private Texture2D _texturePressed;
    private Vector2 _center;
    private float _scale;
    private Color _color;
    private Color _hoverColor;

    private float _layerDepth;

    private string _text;
    private SpriteFont _textFont;
    private Vector2 _textCenter;
    private float _textScale;
    private Color _textColor;
    private Color _textHoverColor;

    private Rectangle _body;
    private bool _buttonPressed, _hovered, _lmbPressed;

    public event EventHandler? Pressed;

    public Button(
        Vector2 position,
        Texture2D textureIdle,
        Texture2D texturePressed,
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
        var ms = Mouse.GetState();
        if (_body.Contains(ms.Position))
        {
            _hovered = true;

            //added controll over prev lmb mouse state to prevent cases when button 
            //wasn't pressed but the lmb was - outside the button - and that still
            //invoked the click event
            if (!_buttonPressed)
            {
                if (!_lmbPressed && ms.LeftButton is ButtonState.Pressed)
                    _buttonPressed = true;
            }
            else
            {
                if (ms.LeftButton == ButtonState.Released)
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

        _lmbPressed = ms.LeftButton is ButtonState.Pressed;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            _buttonPressed ? _texturePressed : _textureIdle,
            _position,
            null,
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