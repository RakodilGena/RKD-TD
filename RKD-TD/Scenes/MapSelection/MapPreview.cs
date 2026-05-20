using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics.Sprites;
using RKD_TD.Models.UI;

namespace RKD_TD.Scenes.MapSelection;

internal sealed class MapPreview
{
    public string Name { get; }
    public string MapFileName { get; }
    private readonly ButtonLabeled _mapDisplay;

    public MapPreview(
        string mapName,
        string mapFileName,
        Vector2 position,
        Vector2 origin,
        Sprite sprite,
        Color idleColor,
        Color hoveredColor,
        Vector2 scale,
        SpriteFont mapNameFont,
        Vector2 mapNameScale,
        Color mapNameColor,
        Color mapNameBorderColor,
        Vector2 mapNameBorderWidth,
        float layerDepth)
    {
        Name = mapName;
        MapFileName = mapFileName;

        _mapDisplay = new ButtonLabeled(
            position,
            origin,
            sprite,
            idleColor,
            hoveredColor,
            scale,
            mapName,
            mapNameFont,
            mapNameScale,
            mapNameColor,
            mapNameBorderColor,
            mapNameBorderWidth,
            layerDepth);

        _mapDisplay.Clicked += (_, args) =>
            Clicked?.Invoke(this, args);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _mapDisplay.Draw(spriteBatch);
    }

    public void Update()
    {
        _mapDisplay.Update();
    }

    public event EventHandler? Clicked;
}