using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Graphics.Sprites;
using MonoGameLibrary.Input;

namespace RKD_TD.Models.UI;

internal sealed class GameCursor
{
    private const int SIZE_X = 50, SIZE_Y = 60;

    private readonly Sprite _idle, _pressed;
    private Vector2 _position;
    private bool _isOnScreen, _isPressed;

    public bool Visible { get; set; } = true;

    public GameCursor(
        TextureRegion idle,
        TextureRegion pressed)
    {
        _idle = new Sprite(idle)
        {
            Scale = new Vector2(
                (float)SIZE_X / idle.Width,
                (float)SIZE_Y / idle.Height)
        };

        _pressed = new Sprite(pressed)
        {
            Scale = new Vector2(
                (float)SIZE_X / pressed.Width,
                (float)SIZE_Y / pressed.Height)
        };

        ShadowSystemMouse();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (_isOnScreen && Visible)
        {
            var current = _isPressed ? _pressed : _idle;

            current.Draw(spriteBatch, _position);
        }
    }

    public void Update()
    {
        ShadowSystemMouse();
    }

    private void ShadowSystemMouse()
    {
        var mouse = Core.Input.Mouse;

        _isPressed = mouse.IsButtonDown(MouseButton.Left);

        if (!mouse.WasMoved)
            return;

        if (!Core.ScreenBounds.Contains(mouse.Position))
        {
            _isOnScreen = false;
            return;
        }

        _isOnScreen = true;
        _position = mouse.Position.ToVector2();
    }
}