using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using RKD_TD.Assets;

namespace RKD_TD.Scenes.MapSelection;

internal sealed class MapSelectionMenu
{
    private readonly MapPreview[] _maps;

    private const int
        MAP_WIDTH = 500,
        MAP_HEIGHT = 300,
        MARGIN_X = 60,
        MARGIN_Y = 50;

    public event EventHandler<MapPreview>? MapClicked;

    public MapSelectionMenu(
        ContentManager content,
        TextureAtlas textures,
        Vector2 position,
        string mapsFileName)
    {
        _maps = InitMaps(content, textures, position, mapsFileName);
    }

    private void SubscribeToMapClicked(MapPreview mapPreview)
    {
        mapPreview.Clicked += (_, _) => MapClicked?.Invoke(this, mapPreview);
    }

    private static MapPreview CreateMap(
        Vector2 menuPosition,
        string mapName,
        SpriteFont mapNameFont,
        string mapFileName,
        int mapIndex,
        Sprite idle,
        Sprite hovered,
        Sprite pressed)
    {
        var mapPosition = GetMapPosition(menuPosition, mapIndex);

        return new MapPreview(
            mapName,
            mapFileName,
            mapPosition,
            origin: Vector2.Zero,
            idle,
            hovered,
            pressed,
            scale: Vector2.One,
            mapNameFont,
            mapNameScale: Vector2.One,
            mapNameColor: Color.Black,
            layerDepth: 1);
    }

    private static MapPreview CreateMockMap(
        Vector2 menuPosition,
        string mapName,
        SpriteFont mapNameFont,
        int mapIndex,
        Sprite idle,
        Sprite hovered,
        Sprite pressed)
    {
        var mapPosition = GetMapPosition(menuPosition, mapIndex);

        return new MapPreview(
            mapName,
            mapFileName: "",
            mapPosition,
            origin: Vector2.Zero,
            idle,
            hovered,
            pressed,
            scale: Vector2.One,
            mapNameFont,
            mapNameScale: Vector2.One,
            mapNameColor: Color.Black,
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

    public void Update()
    {
        foreach (var map in _maps)
        {
            map.Update();
        }
    }

    private MapPreview[] InitMaps(
        ContentManager content,
        TextureAtlas textures,
        Vector2 menuPosition,
        string mapsFileName)
    {
        var spriteIdle = textures.CreateSprite(
            Textures.MapSelection.MAP_BLANK_500_300);
        spriteIdle.Color = Color.Wheat;

        var spriteHovered = textures.CreateSprite(
            Textures.MapSelection.MAP_BLANK_500_300);
        spriteHovered.Color = Color.Gray;

        var spritePressed = textures.CreateSprite(
            Textures.MapSelection.MAP_BLANK_500_300_PRESSED);
        spritePressed.Color = Color.Gray;
        var mapNameFont = GlobalAssets.FontAtlas.GetFont(Fonts.MAP_TITLE);

        var doc = XmlLoader.Load(content, mapsFileName);
        XElement root = doc.Root!;


        const int maxMaps = 6;

        var maps = new List<MapPreview>(maxMaps);

        var regions = root.Elements("Map");

        foreach (var region in regions)
        {
            if (maps.Count >= maxMaps)
                continue;

            string? name = region.Attribute("name")?.Value;
            if (string.IsNullOrEmpty(name))
                continue;

            var mapFileName = region.Attribute("mapFileName")?.Value;
            if (string.IsNullOrEmpty(name))
                continue;

            var textureAlias = region.Attribute("textureAlias")?.Value;
            Sprite idle, hovered, pressed;
            if (string.IsNullOrWhiteSpace(textureAlias))
            {
                (idle, hovered, pressed) = (spriteIdle, spriteHovered, spritePressed);
            }
            else
            {
                idle = textures.CreateSprite(textureAlias);
                idle.Color = Color.White;

                hovered = pressed = textures.CreateSprite(textureAlias);
                pressed.Color = Color.LightGray;
            }

            var map = CreateMap(
                menuPosition,
                name,
                mapNameFont,
                mapFileName!,
                mapIndex: maps.Count,
                idle,
                hovered,
                pressed);

            maps.Add(map);
        }

        while (maps.Count < maxMaps)
        {
            var mapIndex = maps.Count;

            var mapName = $"MOCK MAP {mapIndex + 1}";
            var map = CreateMockMap(
                menuPosition,
                mapName,
                mapNameFont,
                mapIndex,
                spriteIdle,
                spriteHovered,
                spritePressed);

            maps.Add(map);
        }

        foreach (var map in maps)
        {
            SubscribeToMapClicked(map);
        }

        return maps.ToArray();
    }
}