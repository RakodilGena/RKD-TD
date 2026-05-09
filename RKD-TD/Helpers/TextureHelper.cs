using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Graphics.Sprites;

namespace RKD_TD.Helpers;

internal static class TextureHelper
{
    public static (Vector2 scale, Vector2 origin) CalculateScaleAndOrigin(
        float[] size,
        TextureRegion? textureRegion,
        Animation? animation)
    {
        Vector2 scale, origin;
        if (textureRegion is not null)
        {
            scale = new Vector2(
                size[0] / textureRegion.Width,
                size[1] / textureRegion.Height);

            origin = new Vector2(
                textureRegion.Width * 0.5f,
                textureRegion.Height * 0.5f);
        }
        else
        {
            scale = new Vector2(
                size[0] / animation!.Frames[0].Width,
                size[1] / animation.Frames[0].Height);

            origin = new Vector2(
                animation.Frames[0].Width * 0.5f,
                animation.Frames[0].Height * 0.5f);
        }

        return (scale, origin);
    }

    public static Vector2 CalculateScale(
        float[] size,
        TextureRegion? textureRegion,
        Animation? animation)
    {
        Vector2 scale;
        if (textureRegion is not null)
        {
            scale = new Vector2(
                size[0] / textureRegion.Width,
                size[1] / textureRegion.Height);
        }
        else
        {
            scale = new Vector2(
                size[0] / animation!.Frames[0].Width,
                size[1] / animation.Frames[0].Height);
        }

        return scale;
    }
}