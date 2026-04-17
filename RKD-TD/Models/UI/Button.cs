using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RKD_TD.Models.UI;

internal sealed class Button : IMyDrawable
{
    private Vector2 _position;

    private Texture2D _texture;
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
    private float _textLayerDepth;

    private Rectangle _body;
    private bool _pressed, _hovered;

    public Button(
        Vector2 position,
        Texture2D texture,
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
        _texture = texture;
        _center = new Vector2(
            texture.Width / 2, 
            texture.Height /2);
        _scale = scale;
        _color = color;
        _hoverColor = hoverColor;
        _text = text;
        _textFont = textFont;
        _textScale = textScale;
        _textColor = textColor;
        _textHoverColor = textHoverColor;

        _body = new Rectangle(
            (int)(position.X - _texture.Width / 2 * scale),
            (int)(position.Y - _texture.Height / 2 * scale),
            (int)(_texture.Width * scale),
            (int)(_texture.Height * scale));

        _textCenter = _textFont.MeasureString(text) / 2;

        _layerDepth = layerDepth;
        _textLayerDepth = layerDepth + 0.001f;
    }

    public void Update(GameTime gameTime)
    {
        var ms = Mouse.GetState();
        if (_body.Contains(ms.Position))
        {
            _hovered = true;

            if (!_pressed)
            {
                if (ms.LeftButton is ButtonState.Pressed)
                    _pressed = true;
            }
            else
            {
                if (ms.LeftButton == ButtonState.Released)
                {
                    _pressed = false;
                    Console.WriteLine($"Clicked at {gameTime.TotalGameTime.TotalMilliseconds}ms");
                }
                //key pressed;
            }
        }
        else
        {
            _hovered = false;
            _pressed = false;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            _texture,
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