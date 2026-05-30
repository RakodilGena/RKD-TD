using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Visuals;

public static class CameraExtensions
{
    /// <param name="camera"></param>
    extension(ICamera? camera)
    {
        /// <summary>
        /// Use it to discover absolute position of the point with drawing position
        /// </summary>
        /// <returns></returns>
        public Vector2 ScreenToWorld(Vector2 position)
        {
            if (camera is null)
                return position;

            return (position + camera.Position) / camera.Zoom;
        }

        /// <summary>
        /// Use it to discover the drawing position and scale of an element with the absolute position.
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public (Vector2 finalScale, Vector2 finalPosition) WorldToScreen(
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
}