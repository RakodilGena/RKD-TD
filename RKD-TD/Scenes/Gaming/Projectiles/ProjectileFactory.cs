using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Graphics.Sprites;

namespace RKD_TD.Scenes.Gaming.Projectiles;

internal sealed class ProjectileFactory
{
    private readonly FrozenDictionary<string, ProjectileTemplate> _projectiles;

    public ProjectileFactory(FrozenDictionary<string, ProjectileTemplate> projectiles)
    {
        _projectiles = projectiles;
    }

    public Projectile Create(
        string projectileAlias,
        Vector2 position,
        float angle)
    {
        var template = _projectiles[projectileAlias];

        Sprite sprite;
        if (template.Texture != null)
        {
            sprite = new Sprite(template.Texture);
        }
        else
        {
            sprite = new AnimatedSprite(template.Animation!);
        }

        sprite.Scale = template.Scale;
        sprite.Origin = template.Origin;

        return new Projectile(
            sprite,
            template.Speed,
            template.FlightRange,
            template.DirectDamage,
            template.AoeDamage,
            template.AoeRange,
            position,
            rotation: angle);
    }

    public static ProjectileFactory FromFile(
        XDocument turretConfig,
        TextureAtlas gameObjectTextures)
    {
        var projElements = turretConfig.Root!
            .Element("Projectiles")!
            .Elements("Projectile");

        List<ProjectileTemplate> templates = [];

        foreach (var projElement in projElements)
        {
            var projAnimationAlias = projElement.Attribute("animationAlias")?.Value;
            var projTextureAlias = projElement.Attribute("textureAlias")?.Value;


            if (projAnimationAlias is null && projTextureAlias is null
                || projAnimationAlias is not null && projTextureAlias is not null)
                throw new InvalidOperationException("Specify exactly texture or animation.");

            var projTexture = string.IsNullOrWhiteSpace(projTextureAlias)
                ? null
                : gameObjectTextures.GetRegion(projTextureAlias);

            var projAnimation = string.IsNullOrWhiteSpace(projAnimationAlias)
                ? null
                : gameObjectTextures.GetAnimation(projAnimationAlias);

            var alias = projElement.Attribute("alias")?.Value!;
            var projOrigin = ParseToFloatArr(projElement, "origin");
            var projSize = ParseToFloatArr(projElement, "size");
            var projScale = CalculateScale(projSize, projTexture, projAnimation);

            var speed = float.Parse(projElement.Attribute("speed")?.Value!);
            var flightRange = float.Parse(projElement.Attribute("flightRange")?.Value!);
            var directDamage = float.Parse(projElement.Attribute("directDamage")?.Value!);

            var aoeRangeValue = projElement.Attribute("aoeRange")?.Value;
            var aoeRange = !string.IsNullOrEmpty(aoeRangeValue)
                ? float.Parse(aoeRangeValue)
                : 0;

            var aoeDamageValue = projElement.Attribute("aoeDamage")?.Value;
            var aoeDamage = !string.IsNullOrEmpty(aoeDamageValue)
                ? float.Parse(aoeDamageValue)
                : 0;

            templates.Add(
                new ProjectileTemplate(
                    alias,
                    projTexture,
                    projAnimation,
                    Origin: new Vector2(projOrigin[0], projOrigin[1]),
                    projScale,
                    speed,
                    flightRange,
                    directDamage,
                    aoeRange,
                    aoeDamage));
        }

        return new ProjectileFactory(
            templates.ToFrozenDictionary(t => t.Alias));

        static Vector2 CalculateScale(
            float[] size,
            TextureRegion? textureRegion,
            Animation? animation)
        {
            Vector2 scale;
            if (textureRegion is not null)
            {
                scale = new Vector2(
                    size[0] / textureRegion.Width,
                    size[1] / textureRegion.Height);
            }
            else
            {
                scale = new Vector2(
                    size[0] / animation!.Frames[0].Width,
                    size[1] / animation.Frames[0].Height);
            }

            return scale;
        }

        static float[] ParseToFloatArr(XElement element, string attributeName)
        {
            return element.Attribute(attributeName)!.Value
                .Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(float.Parse)
                .ToArray();
        }
    }
}