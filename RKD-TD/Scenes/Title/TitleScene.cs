using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using RKD_TD.Assets;
using RKD_TD.Scenes.MapSelection;

namespace RKD_TD.Scenes.Title;

internal sealed class TitleScene : Scene
{
    private Label _gameTitle = null!;
    private TitleMenu _titleMenu = null!;
    private TextureAtlas _tsAtlas = null!;

    public override void Initialize()
    {
        base.Initialize();

        Core.ExitOnEscape = true;

        InitTitleLabel();
        InitTitleMenu();
    }

    public override void LoadContent()
    {
        base.LoadContent();

        _tsAtlas = TextureAtlas.FromFile(
            Content,
            fileName: "images/main-title/mt-atlas-definition.xml");
    }

    private void InitTitleLabel()
    {
        var screenCenter = Core.GraphicsDevice.Viewport.Width / 2;

        var kwFont180 = GlobalAssets.FontAtlas.GetFont(Fonts.MAIN_TITLE);

        const string mainTitleText = "RKD TOWER DEFENSE";
        var textCenter = kwFont180.MeasureString(mainTitleText) / 2;

        _gameTitle = new Label(
            position: new Vector2(
                screenCenter,
                80),
            origin: new Vector2(
                textCenter.X,
                0),
            mainTitleText,
            kwFont180,
            Color.Black,
            scale: Vector2.One,
            layerDepth: 1);
    }

    private void InitTitleMenu()
    {
        _titleMenu = new TitleMenu(_tsAtlas, new Vector2(65, 435));

        _titleMenu.StartClicked += OnStartClicked;
        _titleMenu.SettingsClicked += OnSettingsClicked;
        _titleMenu.CreditsClicked += OnCreditsClicked;
        _titleMenu.ExitClicked += OnExitClicked;
    }

    private static void OnStartClicked(object? sender, EventArgs eventArgs)
    {
        Console.WriteLine("Start clicked!");
        Core.ChangeScene(new MapSelectionScene());
    }

    private static void OnSettingsClicked(object? sender, EventArgs eventArgs)
    {
        Console.WriteLine("Settings clicked!");
    }

    private static void OnCreditsClicked(object? sender, EventArgs eventArgs)
    {
        Console.WriteLine("Credits clicked!");
    }

    private static void OnExitClicked(object? sender, EventArgs eventArgs)
    {
        Console.WriteLine("Exit clicked!");
        Core.Exit();
    }


    public override void Update(GameTime gameTime)
    {
        _titleMenu.Update(gameTime);

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.LightGray);

        var sb = Core.SpriteBatch;

        sb.Begin();

        _gameTitle.Draw(sb);
        _titleMenu.Draw(sb);

        sb.End();

        base.Draw(gameTime);
    }
}