using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Cameras;
using MonoGameLibrary.Graphics.Sprites;

namespace RKD_TD.Scenes.Gaming.Flashes;

internal class Flash
{
    private readonly Vector2 _position;
    private readonly AnimatedSprite _sprite;
    private readonly float _playTimeSec;

    protected float ElapsedTimeSec { get; private set; }

    public ICamera? Camera
    {
        get => _sprite.Camera;
        set => _sprite.Camera = value;
    }

    public event EventHandler? Finished;

    public Flash(AnimatedSprite sprite, Vector2 position)
    {
        _sprite = sprite;
        _position = position;
        _playTimeSec = (float)(sprite.Animation.Frames.Count * sprite.Animation.Delay.TotalSeconds);
    }

    public void Update(float deltaSeconds)
    {
        ElapsedTimeSec += deltaSeconds;
        _sprite.Update(deltaSeconds);

        if (ElapsedTimeSec < _playTimeSec)
            return;

        Finished?.Invoke(this, EventArgs.Empty);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _sprite.Draw(spriteBatch, _position);
    }
}