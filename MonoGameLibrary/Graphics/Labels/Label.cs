using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics.Labels;

public class Label
{
    public Vector2 Position { get; set; }
    public Vector2 Origin { get; set; } = Vector2.Zero;
    public string Text { get; set; } = "";
    public Color Color { get; set; } = Color.Black;
    public Vector2 Scale { get; set; } = Vector2.One;
    public SpriteFont Font { get; set; }
    public float LayerDepth { get; set; }
    public SpriteEffects Effects { get; set; } = SpriteEffects.None;
    public float Rotation { get; set; } = 0.0f;

    public Label(SpriteFont font)
    {
        Font = font;
    }

    public Label(
        string text,
        SpriteFont font)
    {
        Text = text;
        Font = font;
    }

    public Label(
        Vector2 position,
        Vector2 origin,
        string text,
        SpriteFont font,
        Color color,
        Vector2 scale,
        float layerDepth)
    {
        Position = position;
        Origin = origin;
        Text = text;
        Font = font;
        Color = color;
        Scale = scale;
        LayerDepth = layerDepth;
    }

    /// <summary>
    /// Sets the origin of this sprite to the center.
    /// </summary>
    public void CenterOrigin()
    {
        Origin = Font.MeasureString(Text) / 2;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(
            Font,
            Text,
            Position,
            Color,
            Rotation,
            Origin,
            Scale,
            Effects,
            LayerDepth);
    }
}