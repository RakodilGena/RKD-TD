using System.Diagnostics;
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
    private readonly TextureAtlas _gameObjects;


    private ViewPort _viewPort = null!;


    private Tilemap _map = null!;
    private Portals _portals = null!;


    public GamingScene(string mapFile)
    {
        _mapFile = mapFile;
        _gameObjects = TextureAtlas.FromFile(
            Content,
            fileName: "images/game/game-objects-atlas-definition.xml");
    }

    public override void Initialize()
    {
        base.Initialize();

        _map = Tilemap.FromFile(Content, _mapFile);

        Debug.Assert(_map.Scale == Vector2.One);

        _viewPort = new ViewPort(
            initialZoom: 1,
            maxZoom: 2,
            zoomSpeed: 1f,
            cameraMoveSpeed: 400,
            _map,
            putToCenter: true);

        _portals = Portals.FromFile(Content, _mapFile);

        _map.ViewPort = _viewPort;
        _portals.SetViewPort(_viewPort);
    }

    public override void LoadContent()
    {
        base.LoadContent();
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.LightGray);

        var sb = Core.SpriteBatch;
        sb.Begin();

        _map.Draw(sb);
        _portals.Draw(sb);

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

        _portals.Update(gameTime);

        base.Update(gameTime);
    }
}