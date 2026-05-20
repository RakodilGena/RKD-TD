using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Graphics.Labels;
using MonoGameLibrary.Scenes;
using RKD_TD.Assets;
using RKD_TD.Models.UI;
using RKD_TD.Scenes.Gaming;
using RKD_TD.Scenes.Title;

namespace RKD_TD.Scenes.MapSelection;

internal sealed class MapSelectionScene : Scene
{
    private static readonly Color ButtonIdleColor = Color.DarkGray;
    private static readonly Color ButtonHoveredColor = Color.Gray;

    private Label _selectMapLabel = null!;
    private MapSelectionMenu _mapSelectionMenu = null!;
    private ButtonLabeled _backButton = null!;

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

        _selectMapLabel = new BorderedLabel(
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
            layerDepth: 1,
            borderWidth: new Vector2(2),
            borderColor: Color.White);
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
            ButtonIdleColor,
            ButtonHoveredColor,
            scale: Vector2.One,
            btnText,
            btnFont,
            textScale: Vector2.One,
            textColor: Color.Black,
            borderColor: Color.White,
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

        _selectMapLabel.Draw(sb);
        _mapSelectionMenu.Draw(sb);
        _backButton.Draw(sb);

        sb.End();
        base.Draw(gameTime);
    }
}