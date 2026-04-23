using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

    private ViewPort _viewPort = null!;

    public GamingScene(string mapFile)
    {
        _mapFile = mapFile;
    }

    public override void Initialize()
    {
        base.Initialize();

        var screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        var screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

        var mapWidth = _tilemap.TileWidth * _tilemap.Columns;
        var mapHeight = _tilemap.TileHeight * _tilemap.Rows;

        var screen = new Vector2(screenWidth, screenHeight);
        var map = new Vector2(mapWidth, mapHeight);
        var vpPos = (map - screen) / 2;

        _viewPort = new ViewPort(
            vpPos,
            initialScale: 1,
            minScale: 0.5f,
            maxScale: 2,
            scaleSpeed: 0.6f,
            mapWidth,
            mapHeight,
            cameraMoveSpeed: 300);
        _tilemap.ViewPort = _viewPort;
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

        _viewPort.Update(gameTime);

        base.Update(gameTime);
    }
}