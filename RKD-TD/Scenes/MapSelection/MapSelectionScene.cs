using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using RKD_TD.Assets;
using RKD_TD.Models.UI;
using RKD_TD.Scenes.Gaming;
using RKD_TD.Scenes.Title;

namespace RKD_TD.Scenes.MapSelection;

internal sealed class MapSelectionScene : Scene
{
    private Label _selectMapLabel = null!;
    private MapSelectionMenu _mapSelectionMenu = null!;
    private LabeledButton _backButton = null!;

    private TextureAtlas _msAtlas = null!;

    public override void Initialize()
    {
        base.Initialize();

        Core.ExitOnEscape = false;

        var screenCenter = Core.GraphicsDevice.Viewport.Width / 2;

        InitTitle(screenCenter);
        InitBackButton();
        InitMapSelectionMenu();
    }

    private void InitTitle(int screenCenter)
    {
        var mstFont = GlobalAssets.FontAtlas.GetFont(Fonts.MAP_SELECTION_TITLE);

        const string mstText = "CHOOSE A MAP TO PLAY";
        var textCenter = mstFont.MeasureString(mstText) / 2;

        _selectMapLabel = new Label(
            position: new Vector2(
                screenCenter,
                40),
            origin: new Vector2(
                textCenter.X,
                0),
            mstText,
            mstFont,
            Color.Black,
            scale: Vector2.One,
            layerDepth: 1);
    }

    private void InitBackButton()
    {
        var btnFont = GlobalAssets.FontAtlas.GetFont(Fonts.REGULAR_BTN_TEXT);

        const string btnText = "BACK";

        var spriteIdle = _msAtlas.CreateSprite(
            Textures.MapSelection.BUTTON_300_100);
        spriteIdle.Color = Color.DarkGray;

        var spriteHovered = _msAtlas.CreateSprite(
            Textures.MapSelection.BUTTON_300_100);
        spriteHovered.Color = Color.Gray;

        var spritePressed = _msAtlas.CreateSprite(
            Textures.MapSelection.BUTTON_300_100_PRESSED);
        spritePressed.Color = Color.Gray;

        _backButton = new LabeledButton(
            position: new Vector2(1570, 930),
            origin: Vector2.Zero,
            spriteIdle,
            spriteHovered,
            spritePressed,
            scale: Vector2.One,
            btnText,
            btnFont,
            textScale: Vector2.One,
            textColor: Color.Black,
            layerDepth: 1f);

        _backButton.Clicked += (_, _) => BackToTitle();
    }

    private void InitMapSelectionMenu()
    {
        _mapSelectionMenu = new MapSelectionMenu(
            Content,
            _msAtlas,
            new Vector2(150, 220));

        _mapSelectionMenu.MapClicked += OnMapClicked;
    }

    private static void OnMapClicked(object? sender, MapPreview mapPreview)
    {
        Console.WriteLine($"Map clicked: {mapPreview.Name}");

        if (!string.IsNullOrEmpty(mapPreview.MapFileName))
        {
            Console.WriteLine($"Map file: {mapPreview.MapFileName}");
            Core.ChangeScene(new GamingScene(mapPreview.MapFileName));
        }
    }

    private static void BackToTitle()
    {
        Core.ChangeScene(new TitleScene());
    }

    public override void LoadContent()
    {
        base.LoadContent();

        _msAtlas = TextureAtlas.FromFile(
            Content,
            "images/map-selection/ms-atlas-definition.xml");
    }

    public override void Update(GameTime gameTime)
    {
        _backButton.Update(gameTime);
        _mapSelectionMenu.Update(gameTime);

        HandleEscape();

        base.Update(gameTime);
    }

    private static void HandleEscape()
    {
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Escape))
            BackToTitle();
    }


    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.LightGray);

        var sb = Core.SpriteBatch;
        sb.Begin();

        _selectMapLabel.Draw(sb);
        _mapSelectionMenu.Draw(sb);
        _backButton.Draw(sb);

        sb.End();
        base.Draw(gameTime);
    }
}