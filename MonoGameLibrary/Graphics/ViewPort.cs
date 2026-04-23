using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameLibrary.Graphics;

public sealed class ViewPort : IViewPort
{
    private readonly int _screenWidth, _screenHeight;
    private readonly float _mapWidth, _mapHeight;

    private readonly float _maxScale, _minScale, _scaleSpeed;

    private float _minX,
        _minY,
        _maxX,
        _maxY;

    private float _positionX, _positionY, _cameraMoveSpeed;

    public float PositionX
    {
        get => _positionX;
        set
        {
            if (value < _minX)
                _positionX = _minX;
            else if (value > _maxX)
                _positionX = _maxX;
            else
                _positionX = value;

            //SignalPositionChanged();
        }
    }

    public float PositionY
    {
        get => _positionY;
        set
        {
            if (value < _minY)
                _positionY = _minY;
            else if (value > _maxY)
                _positionY = _maxY;
            else
                _positionY = value;

            //SignalPositionChanged();
        }
    }


    public Vector2 Position => new(_positionX, _positionY);

    public float Scale
    {
        get;
        set
        {
            if (value < _minScale)
                field = _minScale;
            else if (value > _maxScale)
                field = _maxScale;
            else field = value;

            RecalculateMinMaxWidthHeight();
            //ScaleChanged?.Invoke(this, value);
        }
    }


    public ViewPort(
        Vector2 position,
        float initialScale,
        float minScale,
        float maxScale,
        float scaleSpeed,
        float mapWidth,
        float mapHeight,
        float cameraMoveSpeed)
    {
        _minScale = minScale;
        _maxScale = maxScale;
        _scaleSpeed = scaleSpeed;
        _cameraMoveSpeed = cameraMoveSpeed;

        _mapWidth = mapWidth;
        _mapHeight = mapHeight;
        _screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        _screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;


        //no check, recalculated after rescale.
        _positionX = position.X;
        _positionY = position.Y;

        Scale = initialScale;
    }

    private void RecalculateMinMaxWidthHeight()
    {
        SetMinMax(out _minX, out _maxX, _screenWidth, _mapWidth);
        SetMinMax(out _minY, out _maxY, _screenHeight, _mapHeight);

        //reset X Y after recalc because mb they go out of bounds.
        PositionX = _positionX;
        PositionY = _positionY;
    }

    private void SetMinMax(
        out float min,
        out float max,
        int screenDim,
        float mapDim)
    {
        var scaledMapDim = mapDim * Scale;

        var mapMargin = scaledMapDim - screenDim;

        if (mapMargin >= 0)
        {
            min = 0;
            max = mapMargin;
        }
        else
        {
            //mapMargin < 0 meaning map is smaller than screen
            var leftMargin = mapMargin / 2;
            min = max = leftMargin;
        }
    }

    // private void SignalPositionChanged()
    // {
    //     PositionChanged?.Invoke(this, new Vector2(PositionX, PositionY));
    // }


    // public EventHandler<Vector2>? PositionChanged;
    // public EventHandler<float>? ScaleChanged;

    public void Update(GameTime gameTime)
    {
        var gtDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        var scrollWheelDelta = Core.Input.Mouse.ScrollWheelDelta;

        if (scrollWheelDelta != 0)
        {
            HandleScrolled(
                scrollWheelDelta,
                gtDelta);
        }

        HandleMotion(gtDelta);
    }

    private void HandleScrolled(
        int scrollDelta,
        float gtDelta)
    {
        if (scrollDelta < 0)
            Scale -= _scaleSpeed * gtDelta;
        else
            Scale += _scaleSpeed * gtDelta;
    }

    private void HandleMotion(float gtDelta)
    {
        var kb = Core.Input.Keyboard;
        int moveHorizontal = 0, moveVertical = 0;
        if (kb.IsKeyDown(Keys.A))
        {
            moveHorizontal -= 1;
        }

        if (kb.IsKeyDown(Keys.D))
        {
            moveHorizontal += 1;
        }

        if (kb.IsKeyDown(Keys.W))
        {
            moveVertical -= 1;
        }

        if (kb.IsKeyDown(Keys.S))
        {
            moveVertical += 1;
        }

        if (moveHorizontal == 0 && moveVertical == 0)
            return;

        var moveVector = new Vector2(moveHorizontal, moveVertical);
        moveVector.Normalize();

        var moveMultiplier = gtDelta * _cameraMoveSpeed * Scale;

        PositionX += moveVector.X * moveMultiplier;
        PositionY += moveVector.Y * moveMultiplier;
    }
}