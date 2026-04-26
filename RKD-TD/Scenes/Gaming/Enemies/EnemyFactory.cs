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
    private readonly FrozenDictionary<string, EnemyTemplate> _enemyTemplates;
    private readonly WaypointPath _waypointPath;
    private readonly Vector2 _tileSize;

    public EnemyFactory(
        FrozenDictionary<string, EnemyTemplate> enemyTemplates,
        WaypointPath waypointPath,
        Vector2 tileSize)
    {
        _enemyTemplates = enemyTemplates;
        _waypointPath = waypointPath;
        _tileSize = tileSize;
    }

    public Enemy CreateEnemy(string alias)
    {
        var template = _enemyTemplates[alias];
        var sprite = GetSprite(template);
        var positionInTile = GetPositionInTile(sprite);

        return new Enemy(
            template.Health,
            template.Speed,
            template.Reward,
            template.Damage,
            sprite,
            _waypointPath,
            positionInTile);

        Sprite GetSprite(EnemyTemplate tmp)
        {
            if (tmp.Texture is not null)
            {
                sprite = new Sprite(tmp.Texture);
            }
            else
            {
                var randomFrame = Random.Shared.Next(tmp.Animation!.Frames.Count);
                sprite = new AnimatedSprite(tmp.Animation!, currentFrame: randomFrame);
            }

            sprite.Scale = tmp.TextureScale;
            return sprite;
        }

        Vector2 GetPositionInTile(Sprite s)
        {
            var xdiff = _tileSize.X - s.Width;
            var ydiff = _tileSize.Y - s.Height;

            float x = (float)Random.Shared.NextDouble() * xdiff;
            float y = (float)Random.Shared.NextDouble() * ydiff;

            return new Vector2(x, y);
        }
    }

    public static EnemyFactory FromFile(
        XDocument mapDoc,
        TextureAtlas gameObjectsTextures)
    {
        var tileSet = mapDoc.Root!.Element("Tileset")!;
        var tileWidth = int.Parse(tileSet.Attribute("tileWidth")!.Value);
        var tileHeight = int.Parse(tileSet.Attribute("tileHeight")!.Value);
        var tileSize = new Vector2(tileWidth, tileHeight);

        var waypointPath = WaypointPath.FromFile(mapDoc, tileSize);


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
            waypointPath,
            tileSize);

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