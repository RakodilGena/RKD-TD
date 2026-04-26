using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;

namespace RKD_TD.Scenes.Gaming.Enemies;

internal sealed class EnemyFactory
{
    private FrozenDictionary<string, EnemyTemplate> _enemyTemplates;
    private WaypointPath _waypointPath;

    public EnemyFactory(
        FrozenDictionary<string, EnemyTemplate> enemyTemplates,
        WaypointPath waypointPath)
    {
        _enemyTemplates = enemyTemplates;
        _waypointPath = waypointPath;
    }

    public Enemy CreateEnemy(string alias)
    {
        //todo: create actual enemies
        return null!;
    }

    public static EnemyFactory FromFile(
        XDocument mapDoc,
        TextureAtlas gameObjectsTextures)
    {
        var waypointPath = WaypointPath.FromFile(mapDoc);

        var enemies = mapDoc.Root!
            .Element("Spawner")!
            .Elements("Enemies")
            .Elements("Enemy");

        Dictionary<string, EnemyTemplate> templates = [];

        foreach (var enemy in enemies)
        {
            var alias = enemy.Attribute("alias")!.Value;
            var health = int.Parse(enemy.Attribute("health")!.Value);
            var speed = float.Parse(enemy.Attribute("speed")!.Value);
            var cost = int.Parse(enemy.Attribute("cost")!.Value);
            var reward = int.Parse(enemy.Attribute("reward")!.Value);
            var damage = int.Parse(enemy.Attribute("damage")!.Value);

            var size = enemy.Attribute("size")!.Value
                .Split(';')
                .Select(float.Parse)
                .ToArray();

            var textureAlias = enemy.Attribute("textureAlias")?.Value;
            var animationAlias = enemy.Attribute("animationAlias")?.Value;

            if (textureAlias is null && animationAlias is null
                || textureAlias is not null && animationAlias is not null)
                throw new InvalidOperationException("Specify exactly texture or animation.");

            var texture = string.IsNullOrWhiteSpace(textureAlias)
                ? null
                : gameObjectsTextures.GetRegion(textureAlias);

            var animation = string.IsNullOrWhiteSpace(animationAlias)
                ? null
                : gameObjectsTextures.GetAnimation(animationAlias);

            var scale = CalculateScale(size, texture, animation);

            var template = new EnemyTemplate(
                alias,
                health,
                speed,
                cost,
                reward,
                damage,
                scale,
                texture,
                animation);

            templates[alias] = template;
        }

        return new EnemyFactory(
            templates.ToFrozenDictionary(),
            waypointPath);

        static Vector2 CalculateScale(
            float[] size,
            TextureRegion? textureRegion,
            Animation? animation)
        {
            if (textureRegion is not null)
            {
                return new Vector2(
                    size[0] / textureRegion.Width,
                    size[1] / textureRegion.Height);
            }

            return new Vector2(
                size[0] / animation!.Frames[0].Width,
                size[1] / animation.Frames[0].Height);
        }
    }
}