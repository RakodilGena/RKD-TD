using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Visuals;

public interface ICamera
{
    public Vector2 Position { get; }
    public float Zoom { get; }
}