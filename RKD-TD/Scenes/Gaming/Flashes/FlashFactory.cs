using System.Collections.Frozen;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Graphics.Sprites;
using RKD_TD.Helpers;

namespace RKD_TD.Scenes.Gaming.Flashes;

internal sealed class FlashFactory
{
    private readonly FrozenDictionary<string, FlashTemplate> _flashTemplates;

    public FlashFactory(FrozenDictionary<string, FlashTemplate> flashTemplates)
    {
        _flashTemplates = flashTemplates;
    }

    public Flash Create(
        string flashAlias,
        Vector2 position,
        float angle)
    {
        var tmp = _flashTemplates[flashAlias];

        var sprite = new AnimatedSprite(tmp.Animation)
        {
            Rotation = angle,
            Scale = tmp.Scale,
            Origin = tmp.Origin
        };

        return new Flash(
            sprite,
            position);
    }

    public static FlashFactory FromFile(
        XDocument turretConfig,
        TextureAtlas gameObjectTextures)
    {
        var flashElements = turretConfig.Root!
            .Element("Flashes")!
            .Elements("Flash");

        List<FlashTemplate> templates = [];

        foreach (var flashElement in flashElements)
        {
            var alias = flashElement.Attribute("alias")?.Value!;
            var animationAlias = flashElement.Attribute("animationAlias")!.Value;

            var animation = gameObjectTextures.GetAnimation(animationAlias);

            var origin = ParseHelper.ParseToFloatArr(flashElement, "origin", ';');
            var size = ParseHelper.ParseToFloatArr(flashElement, "size", ';');
            var scale = TextureHelper.CalculateScale(size, textureRegion: null, animation);

            templates.Add(
                new FlashTemplate(
                    alias,
                    animation,
                    Origin: new Vector2(origin[0], origin[1]),
                    scale));
        }

        return new FlashFactory(
            templates.ToFrozenDictionary(t => t.Alias));
    }
}