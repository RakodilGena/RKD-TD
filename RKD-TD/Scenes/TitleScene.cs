using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using RKD_TD.Assets;
using RKD_TD.Models.UI;

namespace RKD_TD.Scenes;

internal sealed class TitleScene : Scene
{
    private Label _gameTitle = null!;

    private LabeledButton[] _menuButtons = null!;

    public override void Initialize()
    {
        base.Initialize();

        var screenCenter = Core.GraphicsDevice.Viewport.Width / 2;

        var kwFont180 = GlobalAssets.FontAtlas.GetFont(Fonts.MAIN_TITLE);

        _gameTitle = new Label(
            position: new Vector2(
                screenCenter,
                170),
            "RKD TOWER DEFENSE",
            kwFont180,
            Color.Black,
            scale: 1,
            layerDepth: 1);

        var kwFont90 = GlobalAssets.FontAtlas.GetFont(Fonts.MAIN_MENU_BTN_TEXT);

        var startButtonYPos = 500;
        var menuButtonLeftMargin = 290;
        var buttonsMargin = 150;

        var button450X130 =
            GlobalAssets.TextureAtlas.GetRegion(
                Textures.BUTTON_450_130);
        var button450X130Pressed =
            GlobalAssets.TextureAtlas.GetRegion(
                Textures.BUTTON_450_130_PRESSED);

        var startButton = CreateLabeledButton(
            position: new Vector2(menuButtonLeftMargin, startButtonYPos),
            label: "Start",
            textureIdle: button450X130,
            texturePressed: button450X130Pressed,
            font: kwFont90);

        startButton.Pressed += (sender, args) =>
            Console.WriteLine("Start clicked!");


        var settingsButton = CreateLabeledButton(
            position: new Vector2(menuButtonLeftMargin, startButtonYPos + buttonsMargin),
            label: "Settings",
            textureIdle: button450X130,
            texturePressed: button450X130Pressed,
            font: kwFont90);

        settingsButton.Pressed += (sender, args) =>
            Console.WriteLine("Settings clicked!");


        var creditsButton = CreateLabeledButton(
            position: new Vector2(menuButtonLeftMargin, startButtonYPos + buttonsMargin * 2),
            label: "Credits",
            textureIdle: button450X130,
            texturePressed: button450X130Pressed,
            font: kwFont90);

        creditsButton.Pressed += (sender, args) =>
            Console.WriteLine("Credits clicked!");


        var exitButton = CreateLabeledButton(
            position: new Vector2(menuButtonLeftMargin, startButtonYPos + buttonsMargin * 3),
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
        return new LabeledButton(
            position: position,
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

    public override void Update(GameTime gameTime)
    {
        foreach (var menuButton in _menuButtons)
        {
            menuButton.Update(gameTime);
        }

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.LightGray);

        var sb = Core.SpriteBatch;

        sb.Begin();
        _gameTitle.Draw(sb);

        foreach (var menuButton in _menuButtons)
        {
            menuButton.Draw(sb);
        }

        sb.End();

        base.Draw(gameTime);
    }
}