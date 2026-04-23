using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using RKD_TD.Models.Interfaces;
using RKD_TD.Models.UI;

namespace RKD_TD.Scenes.MapSelection;

internal sealed class MapPreview : IMyDrawable, IMyUpdatable, IMyClickable
{
    public string Name { get; }
    public string MapFileName { get; }
    private readonly LabeledButton _mapDisplay;

    public MapPreview(
        string mapName,
        string mapFileName,
        Vector2 position,
        Vector2 origin,
        Sprite spriteIdle,
        Sprite spriteHovered,
        Sprite spritePressed,
        Vector2 scale,
        SpriteFont mapNameFont,
        Vector2 mapNameScale,
        Color mapNameColor,
        float layerDepth)
    {
        Name = mapName;
        MapFileName = mapFileName;

        _mapDisplay = new LabeledButton(
            position,
            origin,
            spriteIdle,
            spriteHovered,
            spritePressed,
            scale,
            mapName,
            mapNameFont,
            mapNameScale,
            mapNameColor,
            layerDepth);

        _mapDisplay.Clicked += (_, args) =>
            Clicked?.Invoke(this, args);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _mapDisplay.Draw(spriteBatch);
    }

    public void Update(GameTime gameTime)
    {
        _mapDisplay.Update(gameTime);
    }

    public event EventHandler? Clicked;
}