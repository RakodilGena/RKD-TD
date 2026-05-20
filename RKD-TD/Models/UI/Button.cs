using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics.Sprites;
using MonoGameLibrary.Input;

namespace RKD_TD.Models.UI;

public class Button
{
    private readonly Vector2 _position;

    private readonly Sprite _sprite;

    private readonly Color
        _idleColor,
        _hoveredColor;

    private bool _wasPressed, _hovered;
    protected Rectangle Bounds { get; }

    public event EventHandler? Clicked;

    public Button(
        Vector2 position,
        Vector2 origin,
        Sprite sprite,
        Color idleColor,
        Color hoveredColor,
        Vector2 scale,
        float layerDepth)
    {
        _position = position;

        _sprite = sprite;
        _sprite.Color = idleColor;

        _idleColor = idleColor;
        _hoveredColor = hoveredColor;
        _sprite.Origin = origin;
        _sprite.Scale = scale;
        _sprite.LayerDepth = layerDepth;


        Bounds = new Rectangle(
            (int)(position.X - origin.X * scale.X),
            (int)(position.Y - origin.Y * scale.Y),
            (int)_sprite.Width,
            (int)_sprite.Height);
    }

    public void Update()
    {
        var exHovered = _hovered;

        var mouseInfo = Core.Input.Mouse;
        if (Bounds.Contains(mouseInfo.Position))
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

        if (exHovered != _hovered)
            SetColor();
    }

    private void SetColor()
    {
        _sprite.Color = _hovered ? _hoveredColor : _idleColor;
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        _sprite.Draw(
            spriteBatch,
            _position);
    }
}