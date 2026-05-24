using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics.Labels;
using RKD_TD.Assets;

namespace RKD_TD.Scenes.Gaming.Misc;

internal sealed class FpsMeter
{
    private readonly Label _label;

    public FpsMeter()
    {
        var font = GlobalAssets.FontAtlas.GetFont(Fonts.FPS);
        var size = font.MeasureString("FPS: 000");
        
        _label = new Label(font)
        {
            Position = new Vector2(Core.ScreenBounds.Width - size.X, 0),
            Color = Colors.Game.Fps
        };
    }

    public void Update(float deltaSeconds)
    {
        var currentFps = (int)(1f / deltaSeconds);
        _label.Text = $"FPS: {currentFps}";
    }


    public void Draw(SpriteBatch spriteBatch)
    {
        _label.Draw(spriteBatch);
    }
}