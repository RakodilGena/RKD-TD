using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Graphics.Labels;
using MonoGameLibrary.Graphics.Sprites;
using RKD_TD.Assets;
using RKD_TD.Models.UI;

namespace RKD_TD.Scenes.Gaming.PauseMenus;

internal sealed class PauseMenu
{
    private const int
        SIZE_X = 400,
        SIZE_Y = 600,
        POSITION_X = 1920 / 2,
        LABEL_OFFSET_Y = 30,
        LABEL_BORDER_WIDTH = 3,
        BUTTON_SIZE_X = 300,
        BUTTON_SIZE_Y = 100,
        BUTTON_TEXT_BORDER_WIDTH = 2,
        RESUME_BTN_OFFSET_Y = 140,
        EXIT_TO_MAP_SELECTION_BTN_OFFSET_Y = 340,
        EXIT_GAME_BTN_OFFSET_Y = 450;

    private const string
        LABEL_TEXT = "Pause Menu",
        RESUME_BUTTON_TEXT = "Resume",
        EXIT_TO_MAP_SELECTION_BTN_TEXT = "Main Menu",
        EXIT_GAME_BUTTON_TEXT = "Exit Game";


    private readonly Vector2 _position;
    private readonly Sprite _sprite;
    private readonly Label _label;

    private readonly Button
        _resumeButton,
        _exitToMapSelection,
        _exitGame;

    public event EventHandler? Resumed;
    public event EventHandler? ExitedToMainMenu;
    public event EventHandler? ExitedGame;

    public PauseMenu(
        float topY,
        TextureAtlas gameObjectsTextures)
    {
        _sprite = CreateSprite(gameObjectsTextures);
        _position = GetSpritePosition(topY);

        _label = CreateLabel(topY);

        (_resumeButton, _exitToMapSelection, _exitGame) = CreateButtons(topY, gameObjectsTextures);

        _resumeButton.Clicked += (_, _) => Resumed?.Invoke(this, EventArgs.Empty);
        _exitToMapSelection.Clicked += (_, _) => ExitedToMainMenu?.Invoke(this, EventArgs.Empty);
        _exitGame.Clicked += (_, _) => ExitedGame?.Invoke(this, EventArgs.Empty);
    }

    private static Sprite CreateSprite(TextureAtlas gameObjectsTextures)
    {
        var menuTexture = gameObjectsTextures.GetRegion(Textures.Game.PANEL_600_800);
        var menuSprite = new Sprite(menuTexture)
        {
            Origin = new Vector2(menuTexture.Width * 0.5f, 0),
            Scale = new Vector2((float)SIZE_X / menuTexture.Width, (float)SIZE_Y / menuTexture.Height)
        };

        return menuSprite;
    }

    private static Vector2 GetSpritePosition(float topY)
    {
        return new Vector2(POSITION_X, topY);
    }

    private static Label CreateLabel(float topY)
    {
        var position = new Vector2(POSITION_X, topY + LABEL_OFFSET_Y);
        var font = GlobalAssets.FontAtlas.GetFont(Fonts.PAUSE_MENU_TITLE_TEXT);

        var size = font.MeasureString(LABEL_TEXT);

        var label = new BorderedLabel(LABEL_TEXT, font)
        {
            Position = position,
            Color = Colors.Buttons.Text,
            BorderColor = Colors.Buttons.TextBorders,
            BorderWidth = new Vector2(LABEL_BORDER_WIDTH),
            Origin = new Vector2(size.X * 0.5f, 0)
        };
        return label;
    }

    private static (Button resumeButton,
        Button exitToMapSelectionButton,
        Button exitGameButton) CreateButtons(
            float topY,
            TextureAtlas gameObjectsTextures)
    {
        var btnTexture = gameObjectsTextures.GetRegion(Textures.Game.BUTTON);
        var font = GlobalAssets.FontAtlas.GetFont(Fonts.PAUSE_MENU_BTN_TEXT);

        var resumeButton = CreateButton(
            RESUME_BUTTON_TEXT,
            new Vector2(POSITION_X, topY + RESUME_BTN_OFFSET_Y),
            btnTexture,
            font);

        var exitToMapSelectionButton = CreateButton(
            EXIT_TO_MAP_SELECTION_BTN_TEXT,
            new Vector2(POSITION_X, topY + EXIT_TO_MAP_SELECTION_BTN_OFFSET_Y),
            btnTexture,
            font);

        var exitGameButton = CreateButton(
            EXIT_GAME_BUTTON_TEXT,
            new Vector2(POSITION_X, topY + EXIT_GAME_BTN_OFFSET_Y),
            btnTexture,
            font);

        return (resumeButton, exitToMapSelectionButton, exitGameButton);
    }

    private static Button CreateButton(
        string text,
        Vector2 position,
        TextureRegion texture,
        SpriteFont font)
    {
        var origin = new Vector2(texture.Width * 0.5f, 0);
        var scale = new Vector2((float)BUTTON_SIZE_X / texture.Width, (float)BUTTON_SIZE_Y / texture.Height);

        var sprite = new Sprite(texture);

        return new ButtonLabeled(
            position,
            origin,
            sprite,
            Colors.Buttons.Idle,
            Colors.Buttons.Hovered,
            scale,
            text,
            font,
            textScale: Vector2.One,
            textColor: Colors.Buttons.Text,
            borderColor: Colors.Buttons.TextBorders,
            borderWidth: new Vector2(BUTTON_TEXT_BORDER_WIDTH),
            1f);
    }

    public void Draw(SpriteBatch sb)
    {
        _sprite.Draw(sb, _position);
        _label.Draw(sb);

        _resumeButton.Draw(sb);
        _exitToMapSelection.Draw(sb);
        _exitGame.Draw(sb);
    }

    public void Update()
    {
        _sprite.Update(0f);

        _resumeButton.Update();
        _exitToMapSelection.Update();
        _exitGame.Update();
    }
}