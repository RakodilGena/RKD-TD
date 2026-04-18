using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;

namespace RKD_TD.Models.UI;

internal class Button : IMyDrawable
{
    private readonly TextureRegion _textureIdle, _texturePressed;
    private readonly Vector2 _center;
    private readonly float _scale;
    private readonly Color _color, _hoverColor;


    private Rectangle _body;
    private bool _buttonPressed;

    protected Vector2 Position { get; }
    protected float LayerDepth { get; }

    protected bool Hovered { get; private set; }

    public event EventHandler? Pressed;

    public Button(
        Vector2 position,
        TextureRegion textureIdle,
        TextureRegion texturePressed,
        float scale,
        Color color,
        Color hoverColor,
        float layerDepth)
    {
        Position = position;
        _textureIdle = textureIdle;
        _texturePressed = texturePressed;

        _center = new Vector2(
            textureIdle.Width / 2,
            textureIdle.Height / 2);

        _scale = scale;
        _color = color;
        _hoverColor = hoverColor;

        _body = new Rectangle(
            (int)(position.X - textureIdle.Width / 2 * scale),
            (int)(position.Y - textureIdle.Height / 2 * scale),
            (int)(textureIdle.Width * scale),
            (int)(textureIdle.Height * scale));

        LayerDepth = layerDepth;
    }

    public void Update(GameTime gameTime)
    {
        var mouseInfo = Core.Input.Mouse;
        if (_body.Contains(mouseInfo.Position))
        {
            Hovered = true;

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
            Hovered = false;
            _buttonPressed = false;
        }
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        var (texture, srcRect) = _buttonPressed
            ? (_texturePressed.Texture, _texturePressed.SourceRectangle)
            : (_textureIdle.Texture, _textureIdle.SourceRectangle);

        spriteBatch.Draw(
            texture,
            Position,
            srcRect,
            Hovered ? _hoverColor : _color,
            0f,
            origin: _center,
            _scale,
            SpriteEffects.None,
            layerDepth: LayerDepth);
    }
}