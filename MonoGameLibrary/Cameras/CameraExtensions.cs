using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Cameras;

public static class CameraExtensions
{
    public static (Vector2 finalScale, Vector2 finalPosition) Apply(
        this ICamera? camera,
        Vector2 scale,
        Vector2 position)
    {
        if (camera is null)
            return (scale, position);

        var finalPosition = position * camera.Zoom - camera.Position;
        var finalScale = scale * camera.Zoom;

        return (finalScale, finalPosition);
    }
}