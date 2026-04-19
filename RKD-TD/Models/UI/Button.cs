using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;

namespace RKD_TD.Models.UI;

internal class Button : IMyDrawable, IMyUpdatable
{
    private readonly TextureRegion _textureIdle, _texturePressed;
    private readonly Vector2 _origin;
    private readonly float _scale;
    private readonly Color _color, _hoverColor;


    private Rectangle _body;
    private bool _buttonPressed;

    private readonly Vector2 _position;
    protected float LayerDepth { get; }

    protected bool Hovered { get; private set; }

    public event EventHandler? Clicked;

    public Button(
        Vector2 position,
        Vector2 origin,
        TextureRegion textureIdle,
        TextureRegion texturePressed,
        float scale,
        Color color,
        Color hoverColor,
        float layerDepth)
    {
        _position = position;
        _origin = origin;

        _textureIdle = textureIdle;
        _texturePressed = texturePressed;


        _scale = scale;
        _color = color;
        _hoverColor = hoverColor;

        _body = new Rectangle(
            (int)(position.X - origin.X * scale),
            (int)(position.Y - origin.Y * scale),
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
                    Clicked?.Invoke(this, EventArgs.Empty);
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
            _position,
            srcRect,
            Hovered ? _hoverColor : _color,
            0f,
            origin: _origin,
            _scale,
            SpriteEffects.None,
            layerDepth: LayerDepth);
    }
}