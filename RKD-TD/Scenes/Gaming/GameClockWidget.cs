using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using RKD_TD.Assets;
using RKD_TD.Models.UI;

namespace RKD_TD.Scenes.Gaming;

internal sealed class GameClockWidget
{
    private readonly GameClock _gameClock;

    private readonly ButtonToggled
        _pauseBtn,
        _speed1Btn,
        _speed2Btn,
        _speed3Btn;

    private readonly float
        _speed1 = 1,
        _speed2 = 2,
        _speed3 = 3;

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
            _gameClock.SetSpeed(_speed1);

            _pauseBtn.Untoggle();
            _speed2Btn.Untoggle();
            _speed3Btn.Untoggle();
        };

        _speed2Btn.Toggled += (_, _) =>
        {
            _gameClock.Resume();
            _gameClock.SetSpeed(_speed2);

            _speed1Btn.Untoggle();
            _pauseBtn.Untoggle();
            _speed3Btn.Untoggle();
        };

        _speed3Btn.Toggled += (_, _) =>
        {
            _gameClock.Resume();
            _gameClock.SetSpeed(_speed3);

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
        Color idleColor = Color.White,
            hoveredColor = Color.DarkGray,
            toggledColor = Color.Orange,
            toggledHoveredColor = Color.DarkOrange;

        var scale = new Vector2(0.25f);
        var btnMargin = new Vector2(0, 65);

        var sprite = textures.CreateSprite(spriteName);
        sprite.Scale = scale;

        return new ButtonToggled(
            sprite,
            position: widgetPosition + btnMargin * btnIndex,
            idleColor,
            hoveredColor,
            toggledColor,
            toggledHoveredColor);
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

        if (speed == _speed1)
            _speed1Btn.Toggle();
        else if (speed == _speed2)
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