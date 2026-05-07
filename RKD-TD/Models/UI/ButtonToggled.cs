using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics.Sprites;
using MonoGameLibrary.Input;

namespace RKD_TD.Models.UI;

internal sealed class ButtonToggled
{
    private readonly Sprite _sprite;
    private readonly Vector2 _position;
    private Rectangle _bounds;
    private bool _hovered, _toggled;

    private readonly Color _idleColor, _hoveredColor, _toggledColor, _toggledHoveredColor;

    public event EventHandler? Toggled;

    public float LayerDepth
    {
        get => _sprite.LayerDepth;
        set => _sprite.LayerDepth = value;
    }

    public ButtonToggled(
        Sprite sprite,
        Vector2 position,
        Color idleColor,
        Color hoveredColor,
        Color toggledColor,
        Color toggledHoveredColor)
    {
        _sprite = sprite;

        _position = position;
        _idleColor = idleColor;
        _hoveredColor = hoveredColor;
        _toggledColor = toggledColor;
        _toggledHoveredColor = toggledHoveredColor;

        var origin = _sprite.Origin;
        var scale = _sprite.Scale;
        _bounds = new Rectangle(
            (int)(position.X - origin.X * scale.X),
            (int)(position.Y - origin.Y * scale.Y),
            (int)_sprite.Width,
            (int)_sprite.Height);
    }

    public void Update()
    {
        var (exHovered, exToggled) = (_hovered, _toggled);

        var mouseInfo = Core.Input.Mouse;
        if (_bounds.Contains(mouseInfo.Position))
        {
            _hovered = true;

            if (mouseInfo.WasButtonJustPressed(MouseButton.Left) && !_toggled)
            {
                _toggled = true;
                Toggled?.Invoke(this, EventArgs.Empty);
            }
        }
        else
        {
            _hovered = false;
        }

        if (exHovered != _hovered || exToggled != _toggled)
            SetColor();
    }

    private void SetColor()
    {
        _sprite.Color = (_hovered, _toggled) switch
        {
            (true, false) => _hoveredColor,
            (false, true) => _toggledColor,
            (true, true) => _toggledHoveredColor,
            _ => _idleColor
        };
    }

    public void Toggle()
    {
        if (_toggled)
            return;

        _toggled = true;
        SetColor();
        Toggled?.Invoke(this, EventArgs.Empty);
    }

    public void Untoggle()
    {
        _toggled = false;
        SetColor();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _sprite.Draw(spriteBatch, _position);
    }
}