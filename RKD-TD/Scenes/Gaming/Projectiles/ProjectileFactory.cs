using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Graphics.Sprites;
using RKD_TD.Helpers;
using RKD_TD.Scenes.Gaming.Explosions;
using RKD_TD.Scenes.Gaming.Flashes;

namespace RKD_TD.Scenes.Gaming.Projectiles;

internal sealed class ProjectileFactory
{
    private readonly FrozenDictionary<string, ProjectileTemplate> _projectileTemplates;
    private readonly FlashFactory _flashFactory;
    private readonly ExplosionFactory _explosionFactory;

    public ProjectileFactory(
        FrozenDictionary<string, ProjectileTemplate> projectileTemplates,
        FlashFactory flashFactory,
        ExplosionFactory explosionFactory)
    {
        _projectileTemplates = projectileTemplates;
        _explosionFactory = explosionFactory;
        _flashFactory = flashFactory;
    }

    public ProjectileTemplate GetTemplate(string projectileAlias)
    {
        return _projectileTemplates[projectileAlias];
    }

    public Projectile Create(
        ProjectileTemplate template,
        ProjectileValues projectileValues,
        Vector2 position,
        float angle)
    {
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

        if (template.Type is ProjectileType.Homing)
        {
            return new HomingMissile(
                sprite,
                template.Speed,
                projectileValues.FlightRange,
                template.HitCircleRadius,
                projectileValues.DirectDamage,
                projectileValues.AoeDamage,
                projectileValues.AoeRange,
                position,
                rotation: angle,
                template.ExplosionAlias,
                template.TrailFlashAlias,
                template.TrailFlashSpawnPauseSec,
                template.TrailFlashSpawnOffset,
                _explosionFactory,
                _flashFactory);
        }

        return new Projectile(
            sprite,
            template.Speed,
            projectileValues.FlightRange,
            template.HitCircleRadius,
            projectileValues.DirectDamage,
            projectileValues.AoeDamage,
            projectileValues.AoeRange,
            position,
            rotation: angle,
            template.ExplosionAlias,
            template.TrailFlashAlias,
            template.TrailFlashSpawnPauseSec,
            template.TrailFlashSpawnOffset,
            _explosionFactory,
            _flashFactory);
    }

    public Projectile Create(
        string projectileAlias,
        ProjectileValues projectileValues,
        Vector2 position,
        float angle)
    {
        var template = GetTemplate(projectileAlias);

        return Create(template, projectileValues, position, angle);
    }

    public static ProjectileFactory FromFile(
        XDocument turretConfig,
        TextureAtlas gameObjectTextures,
        FlashFactory flashFactory)
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
            var projOrigin = ParseHelper.ParseToFloatArr(projElement, "origin", ';');
            var projSize = ParseHelper.ParseToFloatArr(projElement, "size", ';');
            var projScale = TextureHelper.CalculateScale(projSize, projTexture, projAnimation);

            var speed = float.Parse(projElement.Attribute("speed")?.Value!);
            var hitCircleRadius = int.Parse(projElement.Attribute("hitCircleRadius")!.Value);

            var explosionAlias = projElement.Attribute("explosionAlias")!.Value;

            var typeValue = projElement.Attribute("type")?.Value;
            ProjectileType projType = typeValue switch
            {
                "homing" => ProjectileType.Homing,
                _ => ProjectileType.Standard
            };

            var trailFlashAlias = projElement.Attribute("trailFlashAlias")?.Value;
            float trailFlashSpawnPauseSec;
            Vector2 trailFlashSpawnOffset;

            if (!string.IsNullOrEmpty(trailFlashAlias))
            {
                trailFlashSpawnPauseSec = float.Parse(projElement.Attribute("trailFlashSpawnPauseMs")!.Value) / 1000f;
                var trailFlashSpawnOffsetArr = ParseHelper.ParseToFloatArr(
                    projElement, "trailFlashSpawnOffset", ';');
                trailFlashSpawnOffset = new Vector2(trailFlashSpawnOffsetArr[0], trailFlashSpawnOffsetArr[1]);
            }
            else
            {
                trailFlashSpawnPauseSec = -1f;
                trailFlashSpawnOffset = Vector2.Zero;
            }


            templates.Add(
                new ProjectileTemplate(
                    alias,
                    projTexture,
                    projAnimation,
                    Origin: new Vector2(projOrigin[0], projOrigin[1]),
                    projScale,
                    speed,
                    hitCircleRadius,
                    projType,
                    explosionAlias,
                    trailFlashAlias,
                    trailFlashSpawnPauseSec,
                    trailFlashSpawnOffset
                ));
        }

        var explosionFactory = ExplosionFactory.FromFile(turretConfig, gameObjectTextures);

        return new ProjectileFactory(
            templates.ToFrozenDictionary(t => t.Alias),
            flashFactory,
            explosionFactory);
    }
}