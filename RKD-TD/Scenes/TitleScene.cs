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

        var kwFont120 = GlobalAssets.FontAtlas.GetFont(Fonts.KW120);

        _gameTitle = new Label(
            position: new Vector2(
                screenCenter,
                90),
            "RKD TOWER DEFENSE",
            kwFont120,
            Color.DarkGreen,
            scale: 1,
            layerDepth: 1);

        var kwFont56 = GlobalAssets.FontAtlas.GetFont(Fonts.KW56);
        var topButtonYPos = 140 + 140 + 80 / 2;

        var button300X80 =
            GlobalAssets.TextureAtlas.GetRegion(Textures
                .BUTTON_300_80); //Content.Load<Texture2D>("images/button300x80");
        var button300X80Pressed =
            GlobalAssets.TextureAtlas.GetRegion(Textures
                .BUTTON_300_80_PRESSED); //Content.Load<Texture2D>("images/button300x80p");


        var startButton = CreateLabeledButton(
            position: new Vector2(screenCenter, topButtonYPos),
            label: "Start",
            textureIdle: button300X80,
            texturePressed: button300X80Pressed,
            font: kwFont56);

        startButton.Pressed += (sender, args) =>
            Console.WriteLine("Start clicked!");


        var settingsButton = CreateLabeledButton(
            position: new Vector2(screenCenter, topButtonYPos + 100),
            label: "Settings",
            textureIdle: button300X80,
            texturePressed: button300X80Pressed,
            font: kwFont56);

        settingsButton.Pressed += (sender, args) =>
            Console.WriteLine("Settings clicked!");


        var creditsButton = CreateLabeledButton(
            position: new Vector2(screenCenter, topButtonYPos + 200),
            label: "Credits",
            textureIdle: button300X80,
            texturePressed: button300X80Pressed,
            font: kwFont56);

        creditsButton.Pressed += (sender, args) =>
            Console.WriteLine("Credits clicked!");


        var exitButton = CreateLabeledButton(
            position: new Vector2(screenCenter, topButtonYPos + 300),
            label: "Exit",
            textureIdle: button300X80,
            texturePressed: button300X80Pressed,
            font: kwFont56);

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
            color: Color.White,
            hoverColor: Color.AntiqueWhite,
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
        Core.GraphicsDevice.Clear(Color.PeachPuff);

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