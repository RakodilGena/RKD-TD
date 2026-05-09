using System.Collections.Frozen;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Graphics.Sprites;
using RKD_TD.Helpers;

namespace RKD_TD.Scenes.Gaming.Animations;

internal sealed class GunShotFlashFactory
{
    private readonly FrozenDictionary<string, GunShotFlashTemplate> _flashTemplates;

    public GunShotFlashFactory(FrozenDictionary<string, GunShotFlashTemplate> flashTemplates)
    {
        _flashTemplates = flashTemplates;
    }

    public GunShotFlash Create(
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

        return new GunShotFlash(
            sprite,
            position);
    }

    public static GunShotFlashFactory FromFile(
        XDocument turretConfig,
        TextureAtlas gameObjectTextures)
    {
        var flashElements = turretConfig.Root!
            .Element("Flashes")!
            .Elements("Flash");

        List<GunShotFlashTemplate> templates = [];

        foreach (var flashElement in flashElements)
        {
            var alias = flashElement.Attribute("alias")?.Value!;
            var animationAlias = flashElement.Attribute("animationAlias")!.Value;

            var animation = gameObjectTextures.GetAnimation(animationAlias);

            var origin = ParseHelper.ParseToFloatArr(flashElement, "origin", ';');
            var size = ParseHelper.ParseToFloatArr(flashElement, "size", ';');
            var scale = TextureHelper.CalculateScale(size, textureRegion: null, animation);

            templates.Add(
                new GunShotFlashTemplate(
                    alias,
                    animation,
                    Origin: new Vector2(origin[0], origin[1]),
                    scale));
        }

        return new GunShotFlashFactory(
            templates.ToFrozenDictionary(t => t.Alias));
    }
}