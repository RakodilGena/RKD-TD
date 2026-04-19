using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using RKD_TD.Assets;
using RKD_TD.Models.UI;
using RKD_TD.Scenes.Title;

namespace RKD_TD.Scenes.MapSelection;

internal sealed class MapSelectionScene : Scene
{
    private Label _selectMapLabel = null!;
    private Map[] _maps = [];
    private LabeledButton _backButton = null!;

    private TextureAtlas _msAtlas = null!;

    public override void Initialize()
    {
        base.Initialize();

        Core.ExitOnEscape = false;

        var screenCenter = Core.GraphicsDevice.Viewport.Width / 2;

        InitTitle(screenCenter);
        InitBackButton();
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
            scale: 1,
            layerDepth: 1);
    }

    private void InitBackButton()
    {
        var btnFont = GlobalAssets.FontAtlas.GetFont(Fonts.REGULAR_BTN_TEXT);

        const string btnText = "BACK";

        var textureIdle = _msAtlas.GetRegion("button_300_100");
        var texturePressed = _msAtlas.GetRegion("button_300_100_pressed");

        _backButton = new LabeledButton(
            position: new Vector2(1570, 930),
            origin: Vector2.Zero,
            textureIdle,
            texturePressed,
            scale: 1,
            color: Color.DarkGray,
            hoverColor: Color.Gray,
            btnText,
            btnFont,
            textScale: 1,
            textColor: Color.Black,
            textHoverColor: Color.Black,
            layerDepth: 1f);

        _backButton.Clicked += (_, _) => BackToTitle();
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
        _backButton.Draw(sb);

        sb.End();
        base.Draw(gameTime);
    }
}