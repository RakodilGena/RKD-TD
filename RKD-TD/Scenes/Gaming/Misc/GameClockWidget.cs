using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using RKD_TD.Assets;
using RKD_TD.Models.UI;

namespace RKD_TD.Scenes.Gaming.Misc;

internal sealed class GameClockWidget
{
    private const int
        ICON_SIZE_X = 40,
        ICON_SIZE_Y = 50,
        ICON_MARGIN_Y = 15;

    private readonly GameClock _gameClock;

    public Rectangle Bounds { get; }

    private readonly ButtonToggled
        _pauseBtn,
        _speed1Btn,
        _speed2Btn,
        _speed3Btn;

    private const float
        SPEED1 = 1,
        SPEED2 = 2,
        SPEED3 = 3;

    public float LayerDepth
    {
        get;
        set
        {
            field = value;
            _pauseBtn.LayerDepth = value;
            _speed1Btn.LayerDepth = value;
            _speed2Btn.LayerDepth = value;
            _speed3Btn.LayerDepth = value;
        }
    }

    public GameClockWidget(
        Vector2 widgetPosition,
        TextureAtlas textures)
    {
        //4 buttons, 3 margins
        Bounds = new Rectangle(
            (int)widgetPosition.X,
            (int)widgetPosition.Y,
            ICON_SIZE_X,
            (ICON_SIZE_Y + ICON_MARGIN_Y) * 3 + ICON_SIZE_Y);

        _gameClock = new GameClock();

        _pauseBtn = CreateButton(
            textures,
            widgetPosition,
            spriteName: Textures.Game.PAUSE_150_200,
            btnIndex: 0);

        _speed1Btn = CreateButton(
            textures,
            widgetPosition,
            spriteName: Textures.Game.SPEED1_150_200,
            btnIndex: 1);

        _speed2Btn = CreateButton(
            textures,
            widgetPosition,
            spriteName: Textures.Game.SPEED2_150_200,
            btnIndex: 2);

        _speed3Btn = CreateButton(
            textures,
            widgetPosition,
            spriteName: Textures.Game.SPEED3_150_200,
            btnIndex: 3);

        _pauseBtn.Toggled += (_, _) =>
        {
            _gameClock.Pause();
            _speed1Btn.Untoggle();
            _speed2Btn.Untoggle();
            _speed3Btn.Untoggle();
        };

        _speed1Btn.Toggled += (_, _) =>
        {
            _gameClock.Resume();
            _gameClock.SetSpeed(SPEED1);

            _pauseBtn.Untoggle();
            _speed2Btn.Untoggle();
            _speed3Btn.Untoggle();
        };

        _speed2Btn.Toggled += (_, _) =>
        {
            _gameClock.Resume();
            _gameClock.SetSpeed(SPEED2);

            _speed1Btn.Untoggle();
            _pauseBtn.Untoggle();
            _speed3Btn.Untoggle();
        };

        _speed3Btn.Toggled += (_, _) =>
        {
            _gameClock.Resume();
            _gameClock.SetSpeed(SPEED3);

            _speed1Btn.Untoggle();
            _speed2Btn.Untoggle();
            _pauseBtn.Untoggle();
        };

        _speed1Btn.Toggle();
    }

    private static ButtonToggled CreateButton(
        TextureAtlas textures,
        Vector2 widgetPosition,
        string spriteName,
        int btnIndex)
    {
        var sprite = textures.CreateSprite(spriteName);
        var scale = new Vector2(ICON_SIZE_X / sprite.Width, ICON_SIZE_Y / sprite.Height);
        sprite.Scale = scale;

        var iconMargin = new Vector2(0, ICON_SIZE_Y + ICON_MARGIN_Y) * btnIndex;

        return new ButtonToggled(
            sprite,
            position: widgetPosition + iconMargin,
            Colors.Game.GameClock.Idle,
            Colors.Game.GameClock.Hovered,
            Colors.Game.GameClock.Toggled,
            Colors.Game.GameClock.ToggledHovered);
    }

    public void SwitchPaused()
    {
        if (_gameClock.IsPaused)
            Resume();
        else
            Pause();
    }

    private void Pause()
    {
        _pauseBtn.Toggle();
    }

    private void Resume()
    {
        var speed = _gameClock.TimeScale;

        if (speed is SPEED1)
            _speed1Btn.Toggle();
        else if (speed is SPEED2)
            _speed2Btn.Toggle();
        else
            _speed3Btn.Toggle();
    }

    public void SetSpeed(int speedIndex)
    {
        switch (speedIndex)
        {
            case 1:
                _speed1Btn.Toggle();
                break;
            case 2:
                _speed2Btn.Toggle();
                break;
            case 3:
                _speed3Btn.Toggle();
                break;
        }
    }

    public float GetDelta(GameTime gameTime) => _gameClock.GetDelta(gameTime);

    public void Update()
    {
        _pauseBtn.Update();
        _speed1Btn.Update();
        _speed2Btn.Update();
        _speed3Btn.Update();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _pauseBtn.Draw(spriteBatch);
        _speed1Btn.Draw(spriteBatch);
        _speed2Btn.Draw(spriteBatch);
        _speed3Btn.Draw(spriteBatch);
    }
}