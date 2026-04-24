using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Graphics.Extensions;

public static class ViewPortExtensions
{
    public static (Vector2 finalScale, Vector2 finalPosition) ApplyViewPortToScalePosition(
        this IViewPort? viewPort,
        Vector2 scale,
        Vector2 position)
    {
        if (viewPort is null)
            return (scale, position);

        var finalPosition = position * viewPort.Zoom - viewPort.Position;
        var finalScale = scale * viewPort.Zoom;

        return (finalScale, finalPosition);
    }
}