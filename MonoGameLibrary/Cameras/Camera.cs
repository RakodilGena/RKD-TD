using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Graphics;

namespace MonoGameLibrary.Cameras;

public sealed class Camera : ICamera
{
    private readonly int _screenWidth, _screenHeight;
    private readonly float _mapWidth, _mapHeight, _mapBordersMargin;

    private readonly float _maxZoom, _minZoom, _zoomSpeed, _cameraMoveSpeed;

    private float _minX,
        _minY,
        _maxX,
        _maxY;

    private float _positionX, _positionY;

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
        }
    }


    public Vector2 Position => new(
        _positionX - _mapBordersMargin, 
        _positionY - _mapBordersMargin);

    public float Zoom
    {
        get;
        set
        {
            var oldZoom = field;

            if (value < _minZoom)
                field = _minZoom;
            else if (value > _maxZoom)
                field = _maxZoom;
            else field = value;

            RecalculateMinMaxWidthHeight();
            KeepScreenCentered(oldZoom);
        }
    }


    public Camera(
        float initialZoom,
        float maxZoom,
        float zoomSpeed,
        float cameraMoveSpeed,
        Tilemap tilemap,
        bool putToCenter, 
        float mapBordersMargin)
    {
        if (mapBordersMargin < 0)
            mapBordersMargin = 0;
        // _sideMarginInPx = sideMarginInPx;

        _mapWidth = tilemap.TileWidth * tilemap.Columns + mapBordersMargin * 2;
        _mapHeight = tilemap.TileHeight * tilemap.Rows + mapBordersMargin * 2;

        _screenWidth = Core.GraphicsDevice.Viewport.Width;
        _screenHeight = Core.GraphicsDevice.Viewport.Height;

        var minScaleX = _screenWidth / _mapWidth;
        var minScaleY = _screenHeight / _mapHeight;
        var minScale = Math.Min(minScaleX, minScaleY);
        minScale = Math.Min(minScale, maxZoom);

        _minZoom = minScale;
        _maxZoom = maxZoom;
        _zoomSpeed = zoomSpeed;
        _cameraMoveSpeed = cameraMoveSpeed;
        _mapBordersMargin = mapBordersMargin;

        Zoom = initialZoom;

        if (putToCenter)
        {
            var screen = new Vector2(_screenWidth, _screenHeight);
            var map = new Vector2(_mapWidth, _mapHeight) * Zoom;
            var vpPos = (map - screen) / 2;

            //no check, recalculated after rescale.
            _positionX = vpPos.X;
            _positionY = vpPos.Y;
        }
        else
        {
            _positionX = _minX;
            _positionY = _minY;
        }
    }

    private void RecalculateMinMaxWidthHeight()
    {
        SetMinMax(out _minX, out _maxX, _screenWidth, _mapWidth);
        SetMinMax(out _minY, out _maxY, _screenHeight, _mapHeight);
    }

    private void KeepScreenCentered(float oldZoom)
    {
        var screenCenter = new Vector2(_screenWidth, _screenHeight) / 2;

        var oldCenterPosition = (Position + screenCenter) / oldZoom;

        var newCenterPosition = (Position + screenCenter) / Zoom;

        var centeringDelta = (oldCenterPosition - newCenterPosition) * Zoom;

        PositionX += centeringDelta.X;
        PositionY += centeringDelta.Y;
    }

    private void SetMinMax(
        out float min,
        out float max,
        int screenDim,
        float mapDim)
    {
        var scaledMapDim = mapDim * Zoom;

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

    public void Update(float deltaSeconds)
    {
        var scrollWheelDelta = Core.Input.Mouse.ScrollWheelDelta;

        if (scrollWheelDelta != 0)
        {
            HandleScrolled(
                scrollWheelDelta,
                deltaSeconds);
        }

        HandleMotion(deltaSeconds);
    }

    private void HandleScrolled(
        int scrollDelta,
        float gtDelta)
    {
        if (scrollDelta < 0)
            Zoom -= _zoomSpeed * gtDelta;
        else
            Zoom += _zoomSpeed * gtDelta;
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

        var moveMultiplier = gtDelta * _cameraMoveSpeed * Zoom;

        PositionX += moveVector.X * moveMultiplier;
        PositionY += moveVector.Y * moveMultiplier;
    }
}