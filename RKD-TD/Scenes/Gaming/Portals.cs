using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using RKD_TD.Models.Interfaces;

namespace RKD_TD.Scenes.Gaming;

internal sealed class Portals : IMyDrawable, IMyUpdatable
{
    private readonly RotatingSprite _startingPortal, _endingPortal;
    private readonly Vector2 _startingPortalPosition, _endingPortalPosition;

    private IViewPort? _viewPort;

    public Portals(
        RotatingSprite startingPortal,
        Vector2 startingPortalPosition,
        RotatingSprite endingPortal,
        Vector2 endingPortalPosition)
    {
        _startingPortal = startingPortal;
        _endingPortal = endingPortal;
        _startingPortalPosition = startingPortalPosition;
        _endingPortalPosition = endingPortalPosition;
    }


    public static Portals FromFile(
        ContentManager content,
        string mapFileName)
    {
        string filePath = Path.Combine(content.RootDirectory, mapFileName);

        using var stream = TitleContainer.OpenStream(filePath);
        using var reader = XmlReader.Create(stream);
        var doc = XDocument.Load(reader);
        XElement root = doc.Root!;

        var tileSet = root.Element("Tileset")!;
        var tileWidth = int.Parse(tileSet.Attribute("tileWidth")!.Value);
        var tileHeight = int.Parse(tileSet.Attribute("tileHeight")!.Value);

        string atlasName = root.Element("GameObjectsAtlas")!.Value;
        var textures = TextureAtlas.FromFile(
            content,
            atlasName);

        var toTileCenter = new Vector2(tileWidth * 0.5f, tileHeight * 0.5f
        );

        var portals = root.Element("Portals")!;
        var portalSpriteName = portals.Attribute("textureAlias")!.Value;
        var portalsPositions = portals.Value
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(portalPosition =>
            {
                var split = portalPosition.Split(',');
                if (split.Length != 2)
                    throw new InvalidOperationException("Map Portals invalid config");

                return new Vector2(
                    int.Parse(split[0]) * tileWidth,
                    int.Parse(split[1]) * tileHeight) + toTileCenter;
            }).ToArray();

        var startingPortal = textures.CreateRotatingSprite(portalSpriteName);
        startingPortal.Scale = new Vector2(0.85f);
        startingPortal.Color = Color.LawnGreen;
        startingPortal.RotationDirection = 1;
        startingPortal.RotationSpeedDegreesPerSecond = 10;
        startingPortal.CenterOrigin();

        var endingPortal = textures.CreateRotatingSprite(portalSpriteName);
        endingPortal.Scale = new Vector2(0.85f);
        endingPortal.Color = Color.Red;
        endingPortal.Effects = SpriteEffects.FlipHorizontally;
        endingPortal.RotationDirection = -1;
        endingPortal.RotationSpeedDegreesPerSecond = 10;
        endingPortal.CenterOrigin();

        return new Portals(
            startingPortal,
            startingPortalPosition: portalsPositions[0],
            endingPortal,
            endingPortalPosition: portalsPositions[1]);
    }

    public void SetViewPort(IViewPort viewPort)
    {
        _viewPort = viewPort;

        _startingPortal.ViewPort = viewPort;
        _endingPortal.ViewPort = viewPort;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _startingPortal.Draw(spriteBatch, _startingPortalPosition);
        _endingPortal.Draw(spriteBatch, _endingPortalPosition);
    }

    public void Update(GameTime gameTime)
    {
        _startingPortal.Update(gameTime);
        _endingPortal.Update(gameTime);
    }
}