using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics.Sprites;
using MonoGameLibrary.Visuals;

namespace RKD_TD.Scenes.Gaming.Flashes;

internal class Flash
{
    protected Vector2 Position { get; }
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
        Position = position;
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

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        _sprite.Draw(spriteBatch, Position);
    }
}