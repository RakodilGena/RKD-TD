using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using RKD_TD.Models.Interfaces;
using RKD_TD.Models.UI;

namespace RKD_TD.Scenes.MapSelection;

internal sealed class Map : IMyDrawable, IMyUpdatable, IMyClickable
{
    public string Name { get; }
    private readonly LabeledButton _mapDisplay;

    public Map(
        Vector2 position,
        Vector2 origin,
        Sprite spriteIdle,
        Sprite spriteHovered,
        Sprite spritePressed,
        Vector2 scale,
        string mapName,
        SpriteFont mapNameFont,
        Vector2 mapNameScale,
        Color mapNameColor,
        float layerDepth)
    {
        Name = mapName;

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