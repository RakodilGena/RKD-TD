using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RKD_TD.Models.UI;

namespace RKD_TD;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;

    private Texture2D 
        _title = null!,
        _button300x80 = null!;
    // The Sprite Font reference to draw with
    private SpriteFont _kwFont120= null!, _kwFont80= null!;

    private Button _startButton= null!;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        
        IsFixedTimeStep = true; 
        TargetElapsedTime = TimeSpan.FromSeconds(1d / 60);
    }

    protected override void Initialize()
    {
        // _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        // _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

        
        _graphics.PreferredBackBufferWidth = 1280;  // Desired width
        _graphics.PreferredBackBufferHeight = 720; // Desired height
        _graphics.ApplyChanges();                  // Apply settings to the window
       
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _title = Content.Load<Texture2D>("images/title");
        _button300x80 = Content.Load<Texture2D>("images/button300x80");
        
        _kwFont120 = Content.Load<SpriteFont>("fonts/knightwarrior120");
        _kwFont80 = Content.Load<SpriteFont>("fonts/knightwarrior80");
        
        var screenCenter = _graphics.GraphicsDevice.Viewport.Width / 2;
        var topButtonYPos = 140 + 140 + 80 / 2;
        _startButton = new Button(
            position: new Vector2(screenCenter, topButtonYPos),
            _button300x80,
            scale: 1.5f,
            color: Color.White,
            hoverColor: Color.AntiqueWhite,
            text: "Start",
            _kwFont80,
            textScale: 0.7f,
            textColor: Color.Black,
            textHoverColor: Color.Black,
            layerDepth: 0.5f);
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
        _spriteBatch.Begin();

        // _spriteBatch.Draw(
        //     _title, 
        //     new Rectangle(0,0, 
        //         _graphics.GraphicsDevice.Viewport.Width, 
        //         _graphics.GraphicsDevice.Viewport.Height),
        //     null,
        //     Color.White);

        DrawMenu();

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void DrawMenu()
    {
        var screenCenter = _graphics.GraphicsDevice.Viewport.Width / 2;
        
        var title = "RKD TOWER DEFENCE";
        Vector2 titleCenter = _kwFont120.MeasureString(title) / 2;
        Vector2 titlePosition = new Vector2(
            screenCenter, 
            80);
        
        _spriteBatch.DrawString(
            _kwFont120, 
            title, 
            titlePosition, 
            Color.DarkGreen,
            0, 
            titleCenter, 
            1.0f, 
            SpriteEffects.None, 
            0.5f);


        var topButtonYPos = 140 + 140 + 80 / 2;

        _startButton.Draw(_spriteBatch);
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
        var buttonCenter = new Vector2(300 / 2, 80/2);
        
        _spriteBatch.Draw(
            _button300x80, 
            position,
            null,
            Color.White,
            0f,
            origin: buttonCenter,
            scale:1,
            SpriteEffects.None,
            layerDepth: 0.4f);
        
        Vector2 textCenter = _kwFont80.MeasureString(text) / 2;
        _spriteBatch.DrawString(
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