using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Graphics.Labels;
using MonoGameLibrary.Graphics.Sprites;
using MonoGameLibrary.Scenes;
using RKD_TD.Assets;
using RKD_TD.Scenes.Credits;
using RKD_TD.Scenes.MapSelection;

namespace RKD_TD.Scenes.Title;

internal sealed class TitleScene : Scene
{
    private Sprite _background = null!;
    private Label _gameTitle = null!;
    private TitleMenu _titleMenu = null!;

    private TextureAtlas _tsAtlas = null!;

    public override void Initialize()
    {
        base.Initialize();

        Core.ExitOnEscape = true;

        InitTitleLabel();
        InitTitleMenu();
        InitBackground();

        //free the memory after the atlas no longer needed.
        _tsAtlas = null!;
    }

    public override void LoadContent()
    {
        base.LoadContent();

        _tsAtlas = TextureAtlas.FromFile(
            Content,
            fileName: "images/main-title/mt-atlas-definition.xml");
    }

    private void InitBackground()
    {
        _background = _tsAtlas.CreateSprite(Textures.MainTitle.BACKGROUND);
    }

    private void InitTitleLabel()
    {
        var screenCenter = Core.GraphicsDevice.Viewport.Width / 2;

        var kwFont180 = GlobalAssets.FontAtlas.GetFont(Fonts.MAIN_TITLE);

        const string mainTitleText = "RKD TOWER DEFENSE";
        var textCenter = kwFont180.MeasureString(mainTitleText) / 2;

        _gameTitle = new BorderedLabel(
            position: new Vector2(
                screenCenter,
                0),
            origin: new Vector2(
                textCenter.X,
                0),
            mainTitleText,
            kwFont180,
            color: Colors.MainTitle.TitleColor,
            scale: Vector2.One,
            layerDepth: 1,
            borderColor: Colors.MainTitle.TitleBorders,
            borderWidth: new Vector2(3));
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
        Core.ChangeScene(new MapSelectionScene());
    }

    private static void OnSettingsClicked(object? sender, EventArgs eventArgs)
    {
    }

    private static void OnCreditsClicked(object? sender, EventArgs eventArgs)
    {
        Core.ChangeScene(new CreditsScene());
    }

    private static void OnExitClicked(object? sender, EventArgs eventArgs)
    {
        Core.Exit();
    }


    public override void Update(GameTime gameTime)
    {
        _titleMenu.Update();

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.LightGray);

        var sb = Core.SpriteBatch;

        sb.Begin();

        _background.Draw(sb, new Vector2(0));
        _gameTitle.Draw(sb);
        _titleMenu.Draw(sb);

        sb.End();

        base.Draw(gameTime);
    }
}