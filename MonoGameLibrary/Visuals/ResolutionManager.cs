using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Visuals;

public sealed class ResolutionManager
{
    public static readonly int VirtualWidth = 1920;
    public static readonly int VirtualHeight = 1080;

    public float ScaleX { get; private set; }
    public float ScaleY { get; private set; }
    public float Scale => MathF.Min(ScaleX, ScaleY); // uniform scale - preserves ratio

    // letterbox bars if ratio doesn't match
    public int OffsetX { get; private set; }
    public int OffsetY { get; private set; }

    private RenderTarget2D? _renderTarget;
    private readonly GraphicsDeviceManager _graphics;
    private readonly GraphicsDevice _device;

    public ResolutionManager(GraphicsDeviceManager graphics, GraphicsDevice device)
    {
        _graphics = graphics;
        _device = device;
    }

    public void Initialize()
    {
        _renderTarget = new RenderTarget2D(_device, VirtualWidth, VirtualHeight);

        int screenW = _device.Viewport.Width;
        int screenH = _device.Viewport.Height;

        ScaleX = (float)screenW / VirtualWidth;
        ScaleY = (float)screenH / VirtualHeight;

        // uniform scale preserves aspect ratio - take the smaller to fit entirely
        float uniform = Scale;
        int scaledW = (int)(VirtualWidth * uniform);
        int scaledH = (int)(VirtualHeight * uniform);

        // center the image - this produces letterbox/pillarbox bars
        OffsetX = (screenW - scaledW) / 2;
        OffsetY = (screenH - scaledH) / 2;
    }

    public void BeginCapture()
    {
        _device.SetRenderTarget(_renderTarget);
        _device.Clear(Color.Black);
    }

    public void EndCaptureAndPresent(SpriteBatch spriteBatch)
    {
        _device.SetRenderTarget(null);
        _device.Clear(Color.Black); // clears letterbox bars to black

        float uniform = Scale;

        spriteBatch.Begin(samplerState: SamplerState.LinearClamp);
        spriteBatch.Draw(
            _renderTarget,
            new Rectangle(OffsetX, OffsetY, (int)(VirtualWidth * uniform), (int)(VirtualHeight * uniform)),
            Color.White
        );
        spriteBatch.End();
    }

    public Vector2 ToVirtualMouse(Vector2 screenMouse)
    {
        float uniform = Scale;

        return new Vector2(
            (screenMouse.X - OffsetX) / uniform,
            (screenMouse.Y - OffsetY) / uniform
        );
    }

    public Point ToVirtualMouse(Point screenMouse)
    {
        float uniform = Scale;

        return new Point(
            (int)((screenMouse.X - OffsetX) / uniform),
            (int)((screenMouse.Y - OffsetY) / uniform)
        );
    }
}