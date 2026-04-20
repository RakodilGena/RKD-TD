using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using RKD_TD.Assets;
using RKD_TD.Models.Interfaces;
using RKD_TD.Models.UI;

namespace RKD_TD.Scenes.Title;

public sealed class TitleMenu : IMyDrawable, IMyUpdatable
{
    private readonly LabeledButton[] _menuButtons;

    public event EventHandler?
        StartClicked,
        SettingsClicked,
        CreditsClicked,
        ExitClicked;

    public TitleMenu(
        TextureAtlas textureAtlas,
        Vector2 menuPosition)
    {
        var kwFont90 = GlobalAssets.FontAtlas.GetFont(Fonts.MAIN_MENU_BTN_TEXT);

        var buttonsMargin = 150;

        var startButton = CreateLabeledButton(
            position: menuPosition,
            label: "Start", textureAtlas,
            font: kwFont90);

        startButton.Clicked += (_, args) =>
            StartClicked?.Invoke(this, args);


        var settingsButton = CreateLabeledButton(
            position: menuPosition + new Vector2(0, buttonsMargin),
            label: "Settings", textureAtlas,
            font: kwFont90);

        settingsButton.Clicked += (_, args) =>
            SettingsClicked?.Invoke(this, args);


        var creditsButton = CreateLabeledButton(
            position: menuPosition + new Vector2(0, buttonsMargin * 2),
            label: "Credits", textureAtlas,
            font: kwFont90);

        creditsButton.Clicked += (_, args) =>
            CreditsClicked?.Invoke(this, args);


        var exitButton = CreateLabeledButton(
            position: menuPosition + new Vector2(0, buttonsMargin * 3),
            label: "Exit", textureAtlas,
            font: kwFont90);

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

    private static LabeledButton CreateLabeledButton(
        Vector2 position,
        string label,
        TextureAtlas textureAtlas,
        SpriteFont font)
    {
        var spriteIdle = textureAtlas.CreateSprite(Textures.Title.BUTTON_450_130);
        var spritePressed = textureAtlas.CreateSprite(Textures.Title.BUTTON_450_130_PRESSED);

        return new LabeledButton(
            position: position,
            origin: Vector2.Zero,
            spriteIdle,
            spritePressed,
            scale: Vector2.One,
            colorIdle: Color.DarkGray,
            colorHover: Color.Gray,
            text: label,
            font,
            textScale: 1,
            textColor: Color.Black,
            textHoverColor: Color.Black,
            layerDepth: 0.5f);
    }

    public void Update(GameTime gameTime)
    {
        foreach (var menuButton in _menuButtons)
        {
            menuButton.Update(gameTime);
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