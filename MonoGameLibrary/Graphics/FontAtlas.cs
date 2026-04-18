using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

public sealed class FontAtlas
{
    private readonly Dictionary<string, SpriteFont> _fonts = [];

    public void AddFont(string name, SpriteFont font)
    {
        _fonts[name] = font;
    }

    /// <summary>
    /// Gets the font with the specified name.
    /// </summary>
    /// <param name="name">The name of the font to retrieve.</param>
    /// <returns>The SpriteFont with the specified name.</returns>
    public SpriteFont GetFont(string name)
    {
        return _fonts[name];
    }

    /// <summary>
    /// Removes the font with the specified name.
    /// </summary>
    /// <param name="name">The name of the font to remove.</param>
    /// <returns></returns>
    public bool RemoveFont(string name)
    {
        return _fonts.Remove(name);
    }

    /// <summary>
    /// Creates a new fonts atlas based on a texture atlas xml configuration file.
    /// </summary>
    /// <param name="content">The content manager used to load the fonts for the atlas.</param>
    /// <param name="folderWithFontsName">The path to the folder where xml file and all the fonts lie, relative to the content root directory.</param>
    /// <param name="atlasFileName">The name of the xml-file without path.</param>
    /// <returns>The fonts atlas created by this method.</returns>
    public static FontAtlas FromFile(
        ContentManager content, 
        string folderWithFontsName,
        string atlasFileName)
    {
        string filePath = Path.Combine(content.RootDirectory, folderWithFontsName, atlasFileName);

        using var stream = TitleContainer.OpenStream(filePath);
        using var reader = XmlReader.Create(stream);
        var doc = XDocument.Load(reader);
        XElement root = doc.Root ??
                        throw new InvalidOperationException(
                            "No atlas root was found");

        var atlas = new FontAtlas();

        // The <Fonts> element contains individual <Font> elements,
        // each one describing a different font.
        //
        // Example:
        // <Fonts>
        //      <Font name="Arial" />
        //      <Font name="knightwarrior56"/>
        // </Fonts>
        //
        // So we retrieve all of the <Font> elements then loop through each one
        // and load font and add it to this atlas.
        var regions = root.Element("Fonts")?.Elements("Font");

        if (regions == null)
            return atlas;

        foreach (var region in regions)
        {
            string? fontName = region.Attribute("name")?.Value;
            if (string.IsNullOrEmpty(fontName))
                continue;

            var font = content.Load<SpriteFont>(
                Path.Combine(folderWithFontsName, fontName));

            atlas.AddFont(fontName, font);
        }

        return atlas;
    }
}