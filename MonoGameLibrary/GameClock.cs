using Microsoft.Xna.Framework;

namespace MonoGameLibrary;

public sealed class GameClock
{
    public float TimeScale { get; private set; } = 1f;
    public bool IsPaused { get; private set; }

    public void Pause() => IsPaused = true;
    public void Resume() => IsPaused = false;
    public void SetSpeed(float scale) => TimeScale = scale; // 1f, 2f, 3f

    /// <summary>
    /// Returns elapsed game time, measured in seconds.
    /// </summary>
    /// <param name="gameTime"></param>
    /// <returns></returns>
    public float GetDelta(GameTime gameTime)
    {
        if (IsPaused) return 0f;
        return (float)gameTime.ElapsedGameTime.TotalSeconds * TimeScale;
    }
}