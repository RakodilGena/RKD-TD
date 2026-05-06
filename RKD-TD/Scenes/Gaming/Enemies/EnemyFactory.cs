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
    private readonly Vector2[] _swarmPositions;

    public EnemyFactory(
        FrozenDictionary<string, EnemyTemplate> enemyTemplates,
        WaypointPath waypointPath,
        Vector2 tileSize)
    {
        _enemyTemplates = enemyTemplates;
        _waypointPath = waypointPath;
        _tileSize = tileSize;

        _swarmPositions =
        [
            new Vector2(0, 0),
            new Vector2(0, _tileSize.Y * 0.5f),
            new Vector2(_tileSize.X * 0.5f, 0),
            new Vector2(_tileSize.X * 0.5f, _tileSize.Y * 0.5f),
            new Vector2(_tileSize.X * 0.25f, _tileSize.Y * 0.25f),
        ];
    }

    public Enemy[] CreateEnemy(string alias)
    {
        var template = _enemyTemplates[alias];
        if (template.Type is EnemyType.Regular)
        {
            var sprite = GetSprite(template);

            var positionInTile = GetPositionInTile(
                _tileSize,
                alias,
                sprite);

            var enemy = new Enemy(
                template.Health,
                template.Speed,
                template.Reward,
                template.Damage,
                sprite,
                _waypointPath,
                positionInTile,
                template.Origin,
                template.AppearDistance);

            return [enemy];
        }

        if (template.Type is EnemyType.Swarm)
        {
            var enemies = new Enemy[5];
            for (int i = 0; i < 5; i++)
            {
                var sprite = GetSprite(template);

                var positionInTile =
                    _swarmPositions[i] +
                    GetPositionInTile(
                        freeSpaceForEnemy: _tileSize * 0.5f,
                        alias,
                        sprite);

                var enemy = new Enemy(
                    template.Health,
                    template.Speed,
                    template.Reward,
                    template.Damage,
                    sprite,
                    _waypointPath,
                    positionInTile,
                    template.Origin,
                    template.AppearDistance);

                enemies[i] = enemy;
            }

            return enemies;
        }

        throw new ArgumentOutOfRangeException(nameof(template.Type));
    }

    Sprite GetSprite(EnemyTemplate tmp)
    {
        Sprite sprite;

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
        sprite.Origin = tmp.Origin;

        return sprite;
    }

    private static Vector2 GetPositionInTile(
        Vector2 freeSpaceForEnemy,
        string alias,
        Sprite s)
    {
        var xdiff = freeSpaceForEnemy.X - s.Width;
        var ydiff = freeSpaceForEnemy.Y - s.Height;

        if (xdiff < 0)
            throw new InvalidOperationException(
                $"Wrong [{alias}] enemy setup: sprite width exceeds tile width");

        if (ydiff < 0)
            throw new InvalidOperationException(
                $"Wrong [{alias}] enemy setup: sprite height exceeds tile height");

        float x = (float)Random.Shared.NextDouble() * xdiff;
        float y = (float)Random.Shared.NextDouble() * ydiff;

        return new Vector2(x, y);
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
            var appearDistance = float.Parse(enemy.Attribute("appearDistance")!.Value);

            var customTypeStr = enemy.Attribute("customType")?.Value;
            EnemyType type = customTypeStr switch
            {
                "swarm" => EnemyType.Swarm,
                _ => EnemyType.Regular
            };
            //customType="swarm"

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

            var (scale, origin) = CalculateScaleAndOrigin(size, texture, animation);

            var template = new EnemyTemplate(
                alias,
                health,
                speed,
                cost,
                reward,
                damage,
                appearDistance,
                type,
                scale,
                texture,
                animation,
                origin);

            templates[alias] = template;
        }

        return new EnemyFactory(
            templates.ToFrozenDictionary(),
            waypointPath,
            tileSize);

        static (Vector2 scale, Vector2 origin) CalculateScaleAndOrigin(
            float[] size,
            TextureRegion? textureRegion,
            Animation? animation)
        {
            Vector2 scale, origin;
            if (textureRegion is not null)
            {
                scale = new Vector2(
                    size[0] / textureRegion.Width,
                    size[1] / textureRegion.Height);

                origin = new Vector2(
                    textureRegion.Width * 0.5f,
                    textureRegion.Height * 0.5f);
            }
            else
            {
                scale = new Vector2(
                    size[0] / animation!.Frames[0].Width,
                    size[1] / animation.Frames[0].Height);

                origin = new Vector2(
                    animation.Frames[0].Width * 0.5f,
                    animation.Frames[0].Height * 0.5f);
            }

            return (scale, origin);
        }
    }
}