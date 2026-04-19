using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using RKD_TD.Assets;
using RKD_TD.Models.UI;

namespace RKD_TD.Scenes.Title;

public sealed class TitleMenu : IMyDrawable, IMyUpdatable
{
    private LabeledButton[] _menuButtons;

    public TitleMenu(Vector2 menuPosition)
    {
        var kwFont90 = GlobalAssets.FontAtlas.GetFont(Fonts.MAIN_MENU_BTN_TEXT);

        var buttonsMargin = 150;

        var button450X130 =
            GlobalAssets.TextureAtlas.GetRegion(
                Textures.BUTTON_450_130);
        var button450X130Pressed =
            GlobalAssets.TextureAtlas.GetRegion(
                Textures.BUTTON_450_130_PRESSED);

        var startButton = CreateLabeledButton(
            position: menuPosition,
            label: "Start",
            textureIdle: button450X130,
            texturePressed: button450X130Pressed,
            font: kwFont90);

        startButton.Pressed += (sender, args) =>
            Console.WriteLine("Start clicked!");


        var settingsButton = CreateLabeledButton(
            position: menuPosition + new Vector2(0, buttonsMargin),
            label: "Settings",
            textureIdle: button450X130,
            texturePressed: button450X130Pressed,
            font: kwFont90);

        settingsButton.Pressed += (sender, args) =>
            Console.WriteLine("Settings clicked!");


        var creditsButton = CreateLabeledButton(
            position: menuPosition + new Vector2(0, buttonsMargin * 2),
            label: "Credits",
            textureIdle: button450X130,
            texturePressed: button450X130Pressed,
            font: kwFont90);

        creditsButton.Pressed += (sender, args) =>
            Console.WriteLine("Credits clicked!");


        var exitButton = CreateLabeledButton(
            position: menuPosition + new Vector2(0, buttonsMargin * 3),
            label: "Exit",
            textureIdle: button450X130,
            texturePressed: button450X130Pressed,
            font: kwFont90);

        exitButton.Pressed += (sender, args) =>
        {
            Console.WriteLine("Exit clicked!");
            Core.Exit();
        };

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
        TextureRegion textureIdle,
        TextureRegion texturePressed,
        SpriteFont font)
    {
        //var origin = new Vector2(textureIdle.Width / 2f, textureIdle.Height / 2f);
        return new LabeledButton(
            position: position,
            origin: Vector2.Zero,
            textureIdle,
            texturePressed,
            scale: 1,
            color: Color.DarkGray,
            hoverColor: Color.Gray,
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