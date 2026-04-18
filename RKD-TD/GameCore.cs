using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using RKD_TD.Assets;
using RKD_TD.Models.UI;

namespace RKD_TD;

internal sealed class GameCore : Core
{
    private TextureRegion
        _button300X80 = null!,
        _button300X80Pressed = null!;

    // The Sprite Font reference to draw with
    private SpriteFont _kwFont120 = null!, 
        _kwFont80 = null!, 
        _kwFont56 = null!;

    private Label _gameTitle = null!;
    private LabeledButton _startButton = null!;

    public GameCore() : base(
        title: "RKD Tower Defense",
        width: 1280, //GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
        height: 720, //GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height,
        targetFps: 60,
        fullScreen: false,
        exitOnEscape: true)
    {
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        GlobalAssets.Load(Content);
        
        _button300X80 = GlobalAssets.TextureAtlas.GetRegion(Textures.BUTTON_300_80); //Content.Load<Texture2D>("images/button300x80");
        _button300X80Pressed =
            GlobalAssets.TextureAtlas.GetRegion(Textures.BUTTON_300_80_PRESSED); //Content.Load<Texture2D>("images/button300x80p");

        _kwFont120 = GlobalAssets.FontAtlas.GetFont(Fonts.KW120);
        _kwFont80 = GlobalAssets.FontAtlas.GetFont(Fonts.KW80);
        _kwFont56 = GlobalAssets.FontAtlas.GetFont(Fonts.KW56);

        var screenCenter = GraphicsDevice.Viewport.Width / 2;
        var topButtonYPos = 140 + 140 + 80 / 2;

        _gameTitle = new Label(
            position: new Vector2(
                screenCenter,
                90),
            "RKD TOWER DEFENSE",
            _kwFont120,
            Color.DarkGreen,
            scale: 1,
            layerDepth: 1);

        _startButton = new LabeledButton(
            position: new Vector2(screenCenter, topButtonYPos),
            _button300X80,
            _button300X80Pressed,
            scale: 1,
            color: Color.White,
            hoverColor: Color.AntiqueWhite,
            text: "Start",
            _kwFont56,
            textScale: 1,
            textColor: Color.Black,
            textHoverColor: Color.Black,
            layerDepth: 0.5f);

        _startButton.Pressed += (sender, args) =>
            Console.WriteLine("Clicked!");
    }

    protected override void Update(GameTime gameTime)
    {
        _startButton.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.PeachPuff);

        // Begin the sprite batch to prepare for rendering.
        SpriteBatch.Begin();

        _gameTitle.Draw(SpriteBatch);
        DrawMenu();

        SpriteBatch.End();

        base.Draw(gameTime);
    }

    private void DrawMenu()
    {
        var screenCenter = GraphicsDevice.Viewport.Width / 2;

        var topButtonYPos = 140 + 140 + 80 / 2;

        _startButton.Draw(SpriteBatch);
        // DrawButton(
        //     new Vector2(screenCenter, topButtonYPos),
        //     "Start");

        DrawButton(
            new Vector2(screenCenter, topButtonYPos + 100),
            "Settings");

        DrawButton(
            new Vector2(screenCenter, topButtonYPos + 200),
            "Credits");

        DrawButton(
            new Vector2(screenCenter, topButtonYPos + 300),
            "Exit");
    }

    private void DrawButton(
        Vector2 position,
        string text)
    {
        var buttonCenter = new Vector2(300 / 2, 80 / 2);

        SpriteBatch.Draw(
            _button300X80.Texture,
            position,
            _button300X80.SourceRectangle,
            Color.White,
            0f,
            origin: buttonCenter,
            scale: 1,
            SpriteEffects.None,
            layerDepth: 0.4f);

        Vector2 textCenter = _kwFont80.MeasureString(text) / 2;
        SpriteBatch.DrawString(
            _kwFont80,
            text,
            position,
            Color.Black,
            0,
            textCenter,
            0.7f,
            SpriteEffects.None,
            0.5f);
    }
}