using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using RKD_TD.Scenes.MapSelection;

namespace RKD_TD.Scenes.Gaming;

internal sealed class GamingScene : Scene
{
    //titles and meters

    private readonly string _mapFile;
    private Tilemap _tilemap = null!;

    public GamingScene(string mapFile)
    {
        _mapFile = mapFile;
    }

    public override void LoadContent()
    {
        base.LoadContent();

        _tilemap = Tilemap.FromFile(Content, _mapFile);
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.LightGray);

        var sb = Core.SpriteBatch;
        sb.Begin();

        _tilemap.Draw(sb);

        sb.End();
        base.Draw(gameTime);
    }

    public override void Update(GameTime gameTime)
    {
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Escape))
        {
            Core.ChangeScene(new MapSelectionScene());
        }
        
        base.Update(gameTime);
    }
}