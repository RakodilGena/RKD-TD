using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Graphics.Labels;
using MonoGameLibrary.Graphics.Sprites;
using MonoGameLibrary.Scenes;
using RKD_TD.Assets;
using RKD_TD.Models.UI;
using RKD_TD.Scenes.Title;
using Microsoft.Xna.Framework.Input;

namespace RKD_TD.Scenes.Credits;

internal sealed class CreditsScene : Scene
{
    private Sprite _background = null!;
    private Label _titleLabel = null!;
    private Label[] _creditsLabels = null!;
    private Button _backButton = null!;

    private TextureAtlas _csAtlas = null!;

    public override void LoadContent()
    {
        base.LoadContent();

        _csAtlas = TextureAtlas.FromFile(
            Content,
            "images/credits/credits-atlas-definition.xml");
    }

    public override void Initialize()
    {
        base.Initialize();

        Core.ExitOnEscape = false;

        InitBackground();
        InitTitle();
        InitBackButton();
        InitCredits();

        //free the memory after the atlas no longer needed.
        _csAtlas = null!;
    }

    private void InitBackground()
    {
        _background = _csAtlas.CreateSprite(Textures.Credits.BACKGROUND);
    }

    private void InitTitle()
    {
        var screenCenter = Core.GraphicsDevice.Viewport.Width / 2;

        var mstFont = GlobalAssets.FontAtlas.GetFont(Fonts.SCENE_TITLE);

        const string mstText = "CREDITS";
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

        var sprite = _csAtlas.CreateSprite(
            Textures.Credits.BUTTON_300_100);

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
            borderWidth: new Vector2(2),
            layerDepth: 1f);

        _backButton.Clicked += (_, _) => BackToTitle();
    }


    private void InitCredits()
    {
        var titleFont = GlobalAssets.FontAtlas.GetFont(Fonts.CREDITS_DEPARTMENT_TITLE);
        var textFont = GlobalAssets.FontAtlas.GetFont(Fonts.CREDITS_TEXT);

        Vector2 developmentOffset = new Vector2(300, 400);
        Vector2 artsTitleOffset = new Vector2(1060, 400);
        Vector2 textOffset = new Vector2(0, 100);

        var color = Colors.Credits.Text;
        var borderColor = Colors.Credits.Borders;
        var titleBorderWidth = new Vector2(2.5f);
        var nameBorderWidth = new Vector2(2f);

        var developmentTitle = new BorderedLabel(
            developmentOffset,
            origin: Vector2.Zero,
            "Development",
            titleFont,
            color,
            scale: Vector2.One,
            layerDepth: 1,
            titleBorderWidth,
            borderColor);

        var rkd = new BorderedLabel(
            developmentOffset + textOffset,
            origin: Vector2.Zero,
            "RakodilGena",
            textFont,
            color,
            scale: Vector2.One,
            layerDepth: 1,
            nameBorderWidth,
            borderColor);

        var artsTitle = new BorderedLabel(
            artsTitleOffset,
            origin: Vector2.Zero,
            "Arts and Inspiration",
            titleFont,
            color,
            scale: Vector2.One,
            layerDepth: 1,
            titleBorderWidth,
            borderColor);

        var liza = new BorderedLabel(
            artsTitleOffset + textOffset,
            origin: Vector2.Zero,
            "Liza Baranova",
            textFont,
            color,
            scale: Vector2.One,
            layerDepth: 1,
            nameBorderWidth,
            borderColor);

        _creditsLabels =
        [
            developmentTitle,
            rkd,
            artsTitle,
            liza
        ];
    }

    public override void Update(GameTime gameTime)
    {
        _backButton.Update();

        HandleEscape();

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.LightGray);

        var sb = Core.SpriteBatch;
        sb.Begin();

        _background.Draw(sb, new Vector2(0));
        _titleLabel.Draw(sb);
        foreach (var creditsLabel in _creditsLabels)
        {
            creditsLabel.Draw(sb);
        }

        _backButton.Draw(sb);

        GameCore.Cursor.Draw(sb);

        sb.End();
        base.Draw(gameTime);
    }

    private static void HandleEscape()
    {
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Escape))
            BackToTitle();
    }

    private static void BackToTitle()
    {
        Core.ChangeScene(new TitleScene());
    }
}