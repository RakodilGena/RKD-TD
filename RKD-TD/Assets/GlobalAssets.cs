using Microsoft.Xna.Framework.Content;
using MonoGameLibrary.Graphics;

namespace RKD_TD.Assets;

internal static class GlobalAssets
{
    public static TextureAtlas TextureAtlas { get; private set; } = null!;
    public static FontAtlas FontAtlas { get; private set; } = null!;

    public static void Load(ContentManager content)
    {
        TextureAtlas = TextureAtlas.FromFile(
            content,
            fileName: "images/atlas-definition.xml");

        FontAtlas = FontAtlas.FromFile(
            content,
            folderWithFontsName: "fonts",
            atlasFileName: "font-atlas-definition.xml");
    }
}