using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Graphics.Sprites;
using RKD_TD.Assets;
using RKD_TD.Helpers;
using RKD_TD.Scenes.Gaming.Enemies.HealthBars;

namespace RKD_TD.Scenes.Gaming.Enemies.Spawns;

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

    public EnemyTemplate[] GetEnemyTemplates()
    {
        return _enemyTemplates.Select(t => t.Value).ToArray();
    }

    public Enemy[] CreateEnemy(string alias, int waveIndex)
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
                template,
                sprite,
                _waypointPath,
                positionInTile,
                waveIndex);

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
                    template,
                    sprite,
                    _waypointPath,
                    positionInTile,
                    waveIndex);

                enemies[i] = enemy;
            }

            return enemies;
        }

        throw new ArgumentOutOfRangeException(nameof(template.Type));
    }

    private static Sprite GetSprite(EnemyTemplate tmp)
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
        XDocument enemyConfigDoc,
        XDocument healthBarConfigDoc,
        TextureAtlas gameObjectsTextures)
    {
        var tileSet = mapDoc.Root!.Element("Tileset")!;
        var tileWidth = int.Parse(tileSet.Attribute("tileWidth")!.Value);
        var tileHeight = int.Parse(tileSet.Attribute("tileHeight")!.Value);
        var tileSize = new Vector2(tileWidth, tileHeight);

        var waypointPath = WaypointPath.FromFile(mapDoc, tileSize);

        var enemies = enemyConfigDoc.Root!.Elements("Enemy");

        Dictionary<string, EnemyTemplate> templates = [];

        var healthBarConfig = HealthBarConfig.FromFile(
            healthBarConfigDoc,
            gameObjectsTextures);

        foreach (var enemy in enemies)
        {
            var alias = enemy.Attribute("alias")!.Value;
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

            var size = ParseHelper.ParseToFloatArr(enemy, "size", ';');

            var (scale, origin) = TextureHelper.CalculateScaleAndOrigin(size, texture, animation);

            var hitCircleRadius = int.Parse(enemy.Attribute("hitCircleRadius")!.Value);

            var hitCircleOffsetValue = enemy.Attribute("hitCircleOffset")?.Value;

            Vector2 hitCircleOffset;
            if (!string.IsNullOrWhiteSpace(hitCircleOffsetValue))
            {
                var hitCircleOffsetArr = ParseHelper.ParseToFloatArr(enemy, "hitCircleOffset", ';');
                hitCircleOffset = new Vector2(hitCircleOffsetArr[0], hitCircleOffsetArr[1]);
            }
            else
            {
                hitCircleOffset = Vector2.Zero;
            }

            var health = int.Parse(enemy.Attribute("health")!.Value);
            var healthIncreasePerWave = int.Parse(enemy.Attribute("healthIncreasePerWave")!.Value);
            var healthMultiplierPerWave = float.Parse(enemy.Attribute("healthMultiplierPerWave")!.Value);

            var healthBarTemplate = CreateHealthBarTemplate(
                health,
                healthIncreasePerWave,
                healthMultiplierPerWave,
                size,
                healthBarConfig);

            var isBoss = enemy.Attribute("isBoss")?.Value is "true";

            var template = new EnemyTemplate(
                alias,
                isBoss,
                speed,
                cost,
                reward,
                damage,
                appearDistance,
                type,
                scale,
                texture,
                animation,
                origin,
                hitCircleRadius,
                hitCircleOffset,
                healthBarTemplate);

            templates[alias] = template;
        }

        return new EnemyFactory(
            templates.ToFrozenDictionary(),
            waypointPath,
            tileSize);


        static HealthBarTemplate CreateHealthBarTemplate(
            int health,
            int healthIncreasePerWave,
            float healthMultiplierPerWave,
            float[] enemySize,
            HealthBarConfig cfg)
        {
            var (enemyWidth, enemyHeight) = (enemySize[0], enemySize[1]);

            var width = enemyWidth * cfg.WidthMultiplier;
            var height = width * cfg.HeightMultiplier;

            var enemyOffset = new Vector2(
                -width / 2,
                -enemyHeight * cfg.YOffsetMultiplier);

            var bordersScale = TextureHelper.CalculateScale(
                size: [width, height],
                cfg.BordersTexture,
                animation: null);

            var fillerScale = TextureHelper.CalculateScale(
                size: [width - cfg.Borders.X * 2, height - cfg.Borders.Y * 2],
                cfg.BordersTexture,
                animation: null);

            return new HealthBarTemplate(
                health,
                healthIncreasePerWave,
                healthMultiplierPerWave,
                enemyOffset,
                cfg.Borders,
                cfg.BordersTexture,
                bordersScale,
                BordersColor: Colors.Game.HealthBar.Borders,
                cfg.BackgroundTexture,
                BackgroundColor: Colors.Game.HealthBar.Background,
                cfg.FillerTexture,
                FillerColor: Colors.Game.HealthBar.Filler,
                fillerScale);
        }
    }

    private sealed record HealthBarConfig(
        TextureRegion BordersTexture,
        TextureRegion BackgroundTexture,
        TextureRegion FillerTexture,
        float YOffsetMultiplier,
        Vector2 Borders,
        float WidthMultiplier,
        float HeightMultiplier)
    {
        public static HealthBarConfig FromFile(
            XDocument healthBarCfg,
            TextureAtlas gameObjectsTextures)
        {
            var element = healthBarCfg.Root!;

            var bordersTextureAlias = element.Attribute("bordersTextureAlias")!.Value;
            var bordersTexture = gameObjectsTextures.GetRegion(bordersTextureAlias);

            var backgroundTextureAlias = element.Attribute("backgroundTextureAlias")!.Value;
            var backgroundTexture = gameObjectsTextures.GetRegion(backgroundTextureAlias);


            var fillerTextureAlias = element.Attribute("fillerTextureAlias")!.Value;
            var fillerTexture = gameObjectsTextures.GetRegion(fillerTextureAlias);

            var yOffsetMultiplier = float.Parse(element.Attribute("yOffsetMultiplier")!.Value);
            var widthMultiplier = float.Parse(element.Attribute("widthMultiplier")!.Value);
            var heightMultiplier = float.Parse(element.Attribute("heightMultiplier")!.Value);

            var bordersSize = ParseHelper.ParseToFloatArr(element, "bordersSize", ';');

            return new HealthBarConfig(
                bordersTexture,
                backgroundTexture,
                fillerTexture,
                yOffsetMultiplier,
                Borders: new Vector2(bordersSize[0], bordersSize[1]),
                widthMultiplier,
                heightMultiplier);
        }
    }
}