using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Graphics;

public class RotatingSprite : Sprite
{
    public float RotationDegrees { get; set; }
    public float RotationSpeedDegreesPerSecond { get; set; } = 50;

    public int RotationDirection
    {
        get;
        set
        {
            if (value < 0)
                field = -1;
            else
                field = 1;
        }
    } = 1; // 1 = clockwise, -1 = counter-clockwise

    /// <summary>
    /// Creates a new rotating sprite.
    /// </summary>
    public RotatingSprite()
    {
    }

    public RotatingSprite(
        TextureRegion textureRegion) : base(textureRegion)
    {
    }

    public void Update(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        RotationDegrees += RotationSpeedDegreesPerSecond * RotationDirection * delta;
        RotationDegrees %= 360f;

        Rotation = MathHelper.ToRadians(RotationDegrees);
    }
}