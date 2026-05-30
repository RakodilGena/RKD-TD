using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Graphics.Tiles;
using MonoGameLibrary.Input;

namespace MonoGameLibrary.Visuals;

public sealed class Camera : ICamera
{
    private readonly float _mapWidth, _mapHeight, _mapBordersMargin;

    private readonly float _maxZoom, _minZoom, _zoomSpeed;
    private readonly Vector2 _cameraMoveSpeed;

    private float _minX,
        _minY,
        _maxX,
        _maxY;

    private readonly bool _draggable;
    private bool _isDragged;

    private Vector2 AbsolutePosition
    {
        get;
        set
        {
            var currentX = KeepInBorders(value.X, _minX, _maxX);
            var currentY = KeepInBorders(value.Y, _minY, _maxY);

            field = new Vector2(
                currentX,
                currentY);

            var offset = new Vector2(_mapBordersMargin * Zoom);
            var relativePosition = AbsolutePosition - offset;

            Position = relativePosition;
        }
    }

    private static float KeepInBorders(float value, float min, float max)
    {
        if (value < min)
            return min;
        if (value > max)
            return max;
        return value;
    }

    public Vector2 Position { get; private set; }

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
        //float initialZoom,
        float maxZoom,
        float zoomSpeed,
        float cameraMoveSpeed,
        Tilemap tilemap,
        bool putToCenter,
        float mapBordersMargin,
        float extraBottomMargin,
        bool draggable)
    {
        if (mapBordersMargin < 0)
            mapBordersMargin = 0;
        // _sideMarginInPx = sideMarginInPx;

        _mapWidth = tilemap.TileWidth * tilemap.Columns + mapBordersMargin * 2;
        _mapHeight = tilemap.TileHeight * tilemap.Rows + mapBordersMargin * 2 + extraBottomMargin;

        var screenBounds = Core.VirtualScreenBounds;

        var minScaleX = screenBounds.Width / _mapWidth;
        var minScaleY = screenBounds.Height / _mapHeight;
        var minScale = Math.Min(minScaleX, minScaleY);
        minScale = Math.Min(minScale, maxZoom);

        _minZoom = minScale;
        _maxZoom = maxZoom;
        _zoomSpeed = zoomSpeed;
        _cameraMoveSpeed = new Vector2(cameraMoveSpeed, cameraMoveSpeed);
        _mapBordersMargin = mapBordersMargin;
        _draggable = draggable;

        Zoom = _minZoom;


        if (putToCenter)
        {
            var screen = screenBounds.Size.ToVector2();
            var map = new Vector2(_mapWidth, _mapHeight) * Zoom;
            var vpPos = (map - screen) / 2;

            AbsolutePosition = vpPos;
        }
        else
        {
            AbsolutePosition = new Vector2(_minX, _minY);
        }
    }

    private void RecalculateMinMaxWidthHeight()
    {
        var screenBounds = Core.VirtualScreenBounds;
        SetMinMax(out _minX, out _maxX, screenBounds.Width, _mapWidth);
        SetMinMax(out _minY, out _maxY, screenBounds.Height, _mapHeight);
    }

    private void KeepScreenCentered(float oldZoom)
    {
        var screenCenter = Core.VirtualScreenBounds.Center.ToVector2();

        var oldCenterPosition = (AbsolutePosition + screenCenter) / oldZoom;

        var newCenterPosition = (AbsolutePosition + screenCenter) / Zoom;

        var centeringDelta = (oldCenterPosition - newCenterPosition) * Zoom;

        AbsolutePosition += centeringDelta;
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

        HandleIsDragged();

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

    private void HandleIsDragged()
    {
        if (!_draggable)
            return;

        var mouse = Core.Input.Mouse;
        var virtualPos = Core.Resolution.ToVirtualMouse(mouse.Position);
        if (!Core.VirtualScreenBounds.Contains(virtualPos) || !mouse.IsButtonDown(MouseButton.Middle))
        {
            _isDragged = false;
            return;
        }

        if (mouse.WasButtonJustPressed(MouseButton.Middle))
        {
            _isDragged = true;
        }
    }

    private void HandleMotion(float gameTimeDelta)
    {
        var mouse = Core.Input.Mouse;
        var kb = Core.Input.Keyboard;
        int moveHorizontal = 0, moveVertical = 0;

        if (_draggable && _isDragged)
        {
            var prevAbsolutePosition = AbsolutePosition;

            var desiredDelta = mouse.PositionDelta.ToVector2();
            AbsolutePosition -= desiredDelta / Core.Resolution.Scale;

            //if camera touches the border, it won't move by the desired delta,
            //so we calculate the deltas diff to visually keep the mouse 'glued' to the point
            //by which the camera is getting dragged. 
            var actualDelta = (prevAbsolutePosition - AbsolutePosition) * Core.Resolution.Scale;
            var moveMouse = (actualDelta - desiredDelta).ToPoint();

            if (moveMouse == Point.Zero)
                return;

            mouse.Position += moveMouse;

            return;
        }


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

        var moveMultiplier = gameTimeDelta * _cameraMoveSpeed * Zoom;

        AbsolutePosition += moveVector * moveMultiplier;
    }
}