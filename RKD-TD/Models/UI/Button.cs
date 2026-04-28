using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;

namespace RKD_TD.Models.UI;

internal class Button
{
    private readonly Vector2 _position;

    private readonly Sprite
        _spriteIdle,
        _spriteHovered,
        _spritePressed;

    private Sprite _current;

    private bool _wasPressed, _hovered;
    private Rectangle _bounds;

    public event EventHandler? Clicked;

    public Button(
        Vector2 position,
        Vector2 origin,
        Sprite spriteIdle,
        Sprite spriteHovered,
        Sprite spritePressed,
        Vector2 scale,
        float layerDepth)
    {
        _position = position;
        _current = spriteIdle;

        _spriteIdle = spriteIdle;
        _spriteIdle.Origin = origin;
        _spriteIdle.Scale = scale;
        _spriteIdle.LayerDepth = layerDepth;

        _spriteHovered = spriteHovered;
        _spriteHovered.Origin = origin;
        _spriteHovered.Scale = scale;
        _spriteHovered.LayerDepth = layerDepth;

        _spritePressed = spritePressed;
        _spritePressed.Origin = origin;
        _spritePressed.Scale = scale;
        _spritePressed.LayerDepth = layerDepth;


        _bounds = new Rectangle(
            (int)(position.X - origin.X * scale.X),
            (int)(position.Y - origin.Y * scale.Y),
            (int)_spriteIdle.Width,
            (int)_spriteIdle.Height);
    }

    public void Update(GameTime gameTime)
    {
        var mouseInfo = Core.Input.Mouse;
        if (_bounds.Contains(mouseInfo.Position))
        {
            _hovered = true;

            //added control over prev lmb mouse state to prevent cases when button 
            //wasn't pressed but the lmb was - outside the button - and that still
            //invoked the click event
            if (!_wasPressed)
            {
                if (mouseInfo.WasButtonJustPressed(MouseButton.Left))
                    _wasPressed = true;
            }
            else
            {
                if (mouseInfo.WasButtonJustReleased(MouseButton.Left))
                {
                    _wasPressed = false;
                    Clicked?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        else
        {
            _hovered = false;
            _wasPressed = false;
        }

        SetCurrentSprite();
    }

    private void SetCurrentSprite()
    {
        if (_wasPressed)
            _current = _spritePressed;
        else if (_hovered)
            _current = _spriteHovered;
        else
            _current = _spriteIdle;
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        _current.Draw(
            spriteBatch,
            _position);
    }
}