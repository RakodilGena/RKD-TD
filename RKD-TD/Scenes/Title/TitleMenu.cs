using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Graphics.Sprites;
using RKD_TD.Assets;
using RKD_TD.Models.UI;

namespace RKD_TD.Scenes.Title;

public sealed class TitleMenu
{
    private readonly ButtonLabeled[] _menuButtons;

    public event EventHandler?
        StartClicked,
        SettingsClicked,
        CreditsClicked,
        ExitClicked;

    public TitleMenu(
        TextureAtlas textureAtlas,
        Vector2 menuPosition)
    {
        var buttonsMargin = 150;


        var kwFont90 = GlobalAssets.FontAtlas.GetFont(Fonts.MAIN_MENU_BTN_TEXT);

        var spriteIdle = textureAtlas.CreateSprite(
            Textures.Title.BUTTON_450_130);
        spriteIdle.Color = Color.DarkGray;

        var spriteHovered = textureAtlas.CreateSprite(
            Textures.Title.BUTTON_450_130);
        spriteHovered.Color = Color.Gray;

        var spritePressed = textureAtlas.CreateSprite(
            Textures.Title.BUTTON_450_130_PRESSED);
        spritePressed.Color = Color.Gray;


        var startButton = CreateLabeledButton(
            position: menuPosition,
            label: "Start",
            font: kwFont90,
            spriteIdle,
            spriteHovered,
            spritePressed);

        startButton.Clicked += (_, args) =>
            StartClicked?.Invoke(this, args);


        var settingsButton = CreateLabeledButton(
            position: menuPosition + new Vector2(0, buttonsMargin),
            label: "Settings",
            font: kwFont90,
            spriteIdle,
            spriteHovered,
            spritePressed);

        settingsButton.Clicked += (_, args) =>
            SettingsClicked?.Invoke(this, args);


        var creditsButton = CreateLabeledButton(
            position: menuPosition + new Vector2(0, buttonsMargin * 2),
            label: "Credits",
            font: kwFont90,
            spriteIdle,
            spriteHovered,
            spritePressed);

        creditsButton.Clicked += (_, args) =>
            CreditsClicked?.Invoke(this, args);


        var exitButton = CreateLabeledButton(
            position: menuPosition + new Vector2(0, buttonsMargin * 3),
            label: "Exit",
            font: kwFont90,
            spriteIdle,
            spriteHovered,
            spritePressed);

        exitButton.Clicked += (_, args) =>
            ExitClicked?.Invoke(this, args);

        _menuButtons =
        [
            startButton,
            settingsButton,
            creditsButton,
            exitButton
        ];
    }

    private static ButtonLabeled CreateLabeledButton(
        Vector2 position,
        string label,
        SpriteFont font,
        Sprite idle,
        Sprite hovered,
        Sprite pressed)
    {
        return new ButtonLabeled(
            position: position,
            origin: Vector2.Zero,
            idle,
            hovered,
            pressed,
            scale: Vector2.One,
            text: label,
            font,
            textScale: Vector2.One,
            textColor: Color.Black,
            borderColor: Color.White,
            borderWidth: new Vector2(3, 3),
            layerDepth: 0.5f);
    }

    public void Update()
    {
        foreach (var menuButton in _menuButtons)
        {
            menuButton.Update();
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var menuButton in _menuButtons)
        {
            menuButton.Draw(spriteBatch);
        }
    }
}