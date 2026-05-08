using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics.Labels;

public sealed class BorderedLabel : Label
{
    public Vector2 BorderWidth { get; set; }
    public Color BorderColor { get; set; } = Color.White;

    public BorderedLabel(
        SpriteFont font) : base(font)
    {
    }

    public BorderedLabel(
        string text,
        SpriteFont font) : base(text, font)
    {
    }

    public BorderedLabel(
        Vector2 position,
        Vector2 origin,
        string text,
        SpriteFont font,
        Color color,
        Vector2 scale,
        float layerDepth,
        Vector2 borderWidth,
        Color borderColor)
        : base(position, origin, text, font, color, scale, layerDepth)
    {
        BorderWidth = borderWidth;
        BorderColor = borderColor;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (BorderWidth.X > 0f)
        {
            var horizontalVector = new Vector2(BorderWidth.X, 0f);

            DrawBorder(spriteBatch, horizontalVector);
            DrawBorder(spriteBatch, -horizontalVector);
        }

        if (BorderWidth.Y > 0f)
        {
            var verticalVector = new Vector2(0f, BorderWidth.Y);

            DrawBorder(spriteBatch, verticalVector);
            DrawBorder(spriteBatch, -verticalVector);
        }

        // DrawBorder(spriteBatch, BorderWidth);
        // DrawBorder(spriteBatch, -BorderWidth);
        //
        // DrawBorder(spriteBatch, new Vector2(BorderWidth.X, -BorderWidth.Y));
        // DrawBorder(spriteBatch, new Vector2(-BorderWidth.X, BorderWidth.Y));

        base.Draw(spriteBatch);
    }

    private void DrawBorder(
        SpriteBatch spriteBatch,
        Vector2 offset)
    {
        spriteBatch.DrawString(
            Font,
            Text,
            position: Position + offset,
            BorderColor,
            Rotation,
            Origin,
            Scale,
            Effects,
            LayerDepth);
    }
}