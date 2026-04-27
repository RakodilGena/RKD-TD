using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Cameras;

public interface ICamera
{
    public Vector2 Position { get; }
    public float Zoom { get; }
}