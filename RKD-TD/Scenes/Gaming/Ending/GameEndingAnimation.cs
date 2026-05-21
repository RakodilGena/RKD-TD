using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics.Labels;
using RKD_TD.Assets;

namespace RKD_TD.Scenes.Gaming.Ending;

internal sealed class GameEndingAnimation
{
    private readonly Label _label;

    private const int LABEL_BORDER_WIDTH = 5;

    private const float
        GROW_SECONDS = 1.5f,
        IDLE_SECONDS = 5,
        INITIAL_SCALE = 0,
        FINAL_SCALE = 1,
        SCALE_GROW = (FINAL_SCALE - INITIAL_SCALE) / GROW_SECONDS;

    private float _idleTimer;

    public GameEndingAnimation(
        string text,
        bool isVictory)
    {
        var position = new Vector2(
            GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
            GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height) * 0.5f;

        var color = isVictory
            ? Colors.Game.EndingTitle.Victory
            : Colors.Game.EndingTitle.Defeat;

        var font = GlobalAssets.FontAtlas.GetFont(Fonts.ENDING_TEXT);
        _label = new BorderedLabel(font)
        {
            Text = text,
            Position = position,
            Color = color,
            Scale = new Vector2(INITIAL_SCALE),
            BorderColor = Colors.Game.EndingTitle.Borders,
            BorderWidth = new Vector2(LABEL_BORDER_WIDTH)
        };
        _label.CenterOrigin();
    }

    public bool Update(float deltaSeconds)
    {
        var finalScale = new Vector2(FINAL_SCALE);
        if (_label.Scale.X < finalScale.X)
        {
            var newScale = _label.Scale + new Vector2(SCALE_GROW) * deltaSeconds;
            if (newScale.X > finalScale.X)
            {
                newScale = finalScale;
            }

            _label.Scale = newScale;
            return false;
        }

        if (_idleTimer < IDLE_SECONDS)
        {
            _idleTimer += deltaSeconds;
            return false;
        }

        return true;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _label.Draw(spriteBatch);
    }
}