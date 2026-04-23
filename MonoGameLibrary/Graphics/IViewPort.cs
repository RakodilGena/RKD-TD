using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Graphics;

public interface IViewPort
{
    public Vector2 Position { get; }
    public float Scale { get; }
}