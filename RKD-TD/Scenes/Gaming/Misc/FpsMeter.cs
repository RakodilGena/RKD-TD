using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using RKD_TD.Assets;

namespace RKD_TD.Scenes.Gaming.Misc;

internal sealed class FpsMeter
{
    private readonly Label _label;

    public FpsMeter(Vector2 position)
    {
        var font = GlobalAssets.FontAtlas.GetFont(Fonts.USER_RESOURCES);
        _label = new Label(font)
        {
            Position = position
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