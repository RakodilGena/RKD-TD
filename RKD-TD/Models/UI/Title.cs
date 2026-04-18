using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RKD_TD.Models.UI;

internal sealed class Title : IMyDrawable
{
    private readonly Vector2 _position, _center;
    private readonly string _text;
    private readonly Color _color;
    private readonly float _scale, _layerDepth;
    private readonly SpriteFont _font;

    public Title(
        Vector2 position,
        string text,
        SpriteFont font,
        Color color,
        float scale,
        float layerDepth)
    {
        _position = position;
        _center = font.MeasureString(text) / 2;
        _text = text;
        _font = font;
        _color = color;
        _scale = scale;
        _layerDepth = layerDepth;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(
            _font,
            _text,
            _position,
            _color,
            0,
            _center,
            _scale,
            SpriteEffects.None,
            _layerDepth);
    }
}