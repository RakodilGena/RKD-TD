using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RKD_TD.Models.UI;

internal sealed class Label : IMyDrawable
{
    private readonly Vector2 _position, _origin;
    private readonly string _text;
    private readonly Color _color;
    private readonly float _scale, _layerDepth;
    private readonly SpriteFont _font;

    public Label(
        Vector2 position,
        Vector2 origin,
        string text,
        SpriteFont font,
        Color color,
        float scale,
        float layerDepth)
    {
        _position = position;
        _origin = origin;
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
            rotation: 0,
            _origin,
            _scale,
            SpriteEffects.None,
            _layerDepth);
    }
}