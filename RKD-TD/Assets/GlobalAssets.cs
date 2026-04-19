using Microsoft.Xna.Framework.Content;
using MonoGameLibrary.Graphics;

namespace RKD_TD.Assets;

internal static class GlobalAssets
{
    public static FontAtlas FontAtlas { get; private set; } = null!;

    public static void Load(ContentManager content)
    {
        FontAtlas = FontAtlas.FromFile(
            content,
            folderWithFontsName: "fonts",
            atlasFileName: "font-atlas-definition.xml");
    }
}