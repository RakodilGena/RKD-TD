using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Graphics.Labels;
using MonoGameLibrary.Graphics.Sprites;
using MonoGameLibrary.Scenes;
using RKD_TD.Assets;
using RKD_TD.Models.UI;
using RKD_TD.Scenes.Gaming;
using RKD_TD.Scenes.Title;

namespace RKD_TD.Scenes.MapSelection;

internal sealed class MapSelectionScene : Scene
{
    private Sprite _background = null!;
    private Label _titleLabel = null!;
    private MapSelectionMenu _mapSelectionMenu = null!;
    private Button _backButton = null!;

    private TextureAtlas _msAtlas = null!;

    public override void Initialize()
    {
        base.Initialize();

        Core.ExitOnEscape = false;

        InitBackground();
        InitTitle();
        InitBackButton();
        InitMapSelectionMenu();
    }

    private void InitBackground()
    {
        _background = _msAtlas.CreateSprite(Textures.MapSelection.BACKGROUND);
    }

    private void InitTitle()
    {
        var screenCenter = Core.GraphicsDevice.Viewport.Width / 2;

        var mstFont = GlobalAssets.FontAtlas.GetFont(Fonts.SCENE_TITLE);

        const string mstText = "CHOOSE A MAP TO PLAY";
        var textCenter = mstFont.MeasureString(mstText) / 2;

        _titleLabel = new BorderedLabel(
            position: new Vector2(
                screenCenter,
                40),
            origin: new Vector2(
                textCenter.X,
                0),
            mstText,
            mstFont,
            color: Colors.SceneTitles.Text,
            scale: Vector2.One,
            layerDepth: 1,
            borderWidth: new Vector2(3),
            borderColor: Colors.SceneTitles.Borders);
    }

    private void InitBackButton()
    {
        var btnFont = GlobalAssets.FontAtlas.GetFont(Fonts.REGULAR_BTN_TEXT);

        const string btnText = "BACK";

        var sprite = _msAtlas.CreateSprite(
            Textures.MapSelection.BUTTON_300_100);

        _backButton = new ButtonLabeled(
            position: new Vector2(1570, 930),
            origin: Vector2.Zero,
            sprite,
            Colors.Buttons.Idle,
            Colors.Buttons.Hovered,
            scale: Vector2.One,
            btnText,
            btnFont,
            textScale: Vector2.One,
            textColor: Colors.Buttons.Text,
            borderColor: Colors.Buttons.TextBorders,
            borderWidth: new Vector2(2, 2),
            layerDepth: 1f);

        _backButton.Clicked += (_, _) => BackToTitle();
    }

    private void InitMapSelectionMenu()
    {
        _mapSelectionMenu = new MapSelectionMenu(
            Content,
            _msAtlas,
            new Vector2(150, 220),
            mapsFileName: "maps/maps.xml");

        _mapSelectionMenu.MapClicked += OnMapClicked;
    }

    private static void OnMapClicked(object? sender, MapPreview mapPreview)
    {
        if (string.IsNullOrEmpty(mapPreview.MapFileName))
            return;

        Core.ChangeScene(new GamingScene(mapPreview.MapFileName));
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
        _backButton.Update();
        _mapSelectionMenu.Update();

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

        _background.Draw(sb, new Vector2(0));
        _titleLabel.Draw(sb);
        _mapSelectionMenu.Draw(sb);
        _backButton.Draw(sb);

        sb.End();
        base.Draw(gameTime);
    }
}