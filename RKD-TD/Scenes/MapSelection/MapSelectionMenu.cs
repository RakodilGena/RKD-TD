using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using RKD_TD.Assets;
using RKD_TD.Models.Interfaces;

namespace RKD_TD.Scenes.MapSelection;

internal sealed class MapSelectionMenu : IMyDrawable, IMyUpdatable
{
    private readonly Map[] _maps;

    private const int
        MAP_WIDTH = 500,
        MAP_HEIGHT = 300,
        MARGIN_X = 60,
        MARGIN_Y = 50;

    public event EventHandler<Map>? MapClicked;

    public MapSelectionMenu(
        TextureAtlas textures,
        Vector2 position)
    {
        var mapNameFont = GlobalAssets.FontAtlas.GetFont(Fonts.MAP_TITLE);

        var mapsCount = 6;
        _maps = new Map[mapsCount];
        for (int mapIndex = 0; mapIndex < mapsCount; mapIndex++)
        {
            var mapName = $"MOCK MAP {mapIndex + 1}";
            var map = CreateMockMap(
                position,
                textures,
                mapNameFont,
                mapName,
                mapIndex);

            _maps[mapIndex] = map;
            SubscribeToMapClicked(map);
        }
    }

    private void SubscribeToMapClicked(Map map)
    {
        map.Clicked += (_, _) => MapClicked?.Invoke(this, map);
    }

    private static Map CreateMockMap(
        Vector2 menuPosition,
        TextureAtlas textures,
        SpriteFont mapNameFont,
        string mapName,
        int mapIndex)
    {
        var mapPosition = GetMapPosition(menuPosition, mapIndex);
        
        var spriteIdle = textures.CreateSprite(
            Textures.MapSelection.MAP_BLANK_500_300);
        var spritePressed = textures.CreateSprite(
            Textures.MapSelection.MAP_BLANK_500_300_PRESSED);

        return new Map(
            mapPosition,
            origin: Vector2.Zero,
            spriteIdle,
            spritePressed,
            scale: Vector2.One,
            color: Color.White,
            hoverColor: Color.Gray,
            mapName,
            mapNameFont,
            mapNameScale: 1,
            mapNameColor: Color.Black,
            mapNameHoverColor: Color.Black,
            layerDepth: 1);
    }

    private static Vector2 GetMapPosition(
        Vector2 menuPosition,
        int mapIndex)
    {
        var x = (mapIndex % 3) * (MAP_WIDTH + MARGIN_X);
        var y = (mapIndex / 3) * (MAP_HEIGHT + MARGIN_Y);

        return new Vector2(x, y) + menuPosition;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var map in _maps)
        {
            map.Draw(spriteBatch);
        }
    }

    public void Update(GameTime gameTime)
    {
        foreach (var map in _maps)
        {
            map.Update(gameTime);
        }
    }
}