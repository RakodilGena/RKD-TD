using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using RKD_TD.Models.UI;

namespace RKD_TD;

public class Game1 : Core
{
    private TextureRegion
        _button300X80 = null!,
        _button300X80Pressed = null!;

    // The Sprite Font reference to draw with
    private SpriteFont _kwFont120 = null!, _kwFont80 = null!;

    private Button _startButton = null!;
    private Title _gameTitle = null!;

    public Game1() : base(
        title: "RKD Tower Defense",
        width: 1280, //GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
        height: 720, //GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height,
        targetFps: 60,
        fullScreen: false)
    {
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        var atlas = TextureAtlas.FromFile(Content, "images/atlas-definition.xml");

        _button300X80 = atlas.GetRegion("button300x80"); //Content.Load<Texture2D>("images/button300x80");
        _button300X80Pressed =
            atlas.GetRegion("button300x80pressed"); //Content.Load<Texture2D>("images/button300x80p");

        _kwFont120 = Content.Load<SpriteFont>("fonts/knightwarrior120");
        _kwFont80 = Content.Load<SpriteFont>("fonts/knightwarrior80");

        var screenCenter = GraphicsDevice.Viewport.Width / 2;
        var topButtonYPos = 140 + 140 + 80 / 2;

        _gameTitle = new Title(
            position: new Vector2(
                screenCenter,
                90),
            "RKD TOWER DEFENSE",
            _kwFont120,
            Color.DarkGreen,
            scale: 1,
            layerDepth: 1);

        _startButton = new Button(
            position: new Vector2(screenCenter, topButtonYPos),
            _button300X80,
            _button300X80Pressed,
            scale: 1,
            color: Color.White,
            hoverColor: Color.AntiqueWhite,
            text: "Start",
            _kwFont80,
            textScale: 0.7f,
            textColor: Color.Black,
            textHoverColor: Color.Black,
            layerDepth: 0.5f);

        _startButton.Pressed += (sender, args) =>
            Console.WriteLine("Clicked!");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

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