using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using RKD_TD.Models.Interfaces;

namespace RKD_TD.Models.UI;

internal class Button : IMyDrawable, IMyUpdatable, IMyClickable
{
    private readonly Vector2 _position;
    private readonly Sprite _spriteIdle, _spritePressed;
    private bool _inPressedState;
    protected bool Hovered { get; private set; }

    private readonly Color _colorIdle, _colorHovered;
    private Rectangle _body;
    
    protected float LayerDepth => _spriteIdle.LayerDepth;

    public event EventHandler? Clicked;

    public Button(
        Vector2 position,
        Vector2 origin,
        Sprite spriteIdle,
        Sprite spritePressed,
        Vector2 scale,
        Color colorIdle,
        Color colorHover,
        float layerDepth)
    {
        _position = position;
        
        _spriteIdle = spriteIdle;
        _spriteIdle.Origin = origin;
        _spriteIdle.Scale = scale;
        _spriteIdle.LayerDepth = layerDepth;
        
        _spritePressed = spritePressed;
        _spritePressed.Origin = origin;
        _spritePressed.Scale = scale;
        _spritePressed.LayerDepth = layerDepth;

        _colorIdle = colorIdle;
        _colorHovered = colorHover;

        _body = new Rectangle(
            (int)(position.X - origin.X * scale.X),
            (int)(position.Y - origin.Y * scale.Y),
            (int)_spriteIdle.Width,
            (int)_spriteIdle.Height);
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
            if (!_inPressedState)
            {
                if (mouseInfo.WasButtonJustPressed(MouseButton.Left))
                    _inPressedState = true;
            }
            else
            {
                if (mouseInfo.WasButtonJustReleased(MouseButton.Left))
                {
                    _inPressedState = false;
                    Clicked?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        else
        {
            Hovered = false;
            _inPressedState = false;
        }
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        var spriteToDraw = _inPressedState
            ? _spritePressed
            : _spriteIdle;
        
        var colorToDraw = Hovered
            ? _colorHovered 
            : _colorIdle;
        spriteToDraw.Color = colorToDraw;
        
        spriteToDraw.Draw(
            spriteBatch,
            _position);
    }
}