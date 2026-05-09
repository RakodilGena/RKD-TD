using System.Collections.Frozen;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Graphics.Sprites;
using RKD_TD.Helpers;

namespace RKD_TD.Scenes.Gaming.Explosions;

internal sealed class ExplosionFactory
{
    private readonly FrozenDictionary<string, ExplosionTemplate> _explosionTemplates;

    public ExplosionFactory(FrozenDictionary<string, ExplosionTemplate> explosionTemplates)
    {
        _explosionTemplates = explosionTemplates;
    }

    public Explosion Create(
        string explosionAlias,
        Vector2 position,
        int aoeRange,
        int aoeDamage)
    {
        var tmp = _explosionTemplates[explosionAlias];

        var sprite = new AnimatedSprite(tmp.Animation)
        {
            Scale = tmp.Scale,
            Origin = tmp.Origin
        };

        return new Explosion(
            sprite,
            position,
            aoeRange,
            aoeDamage,
            tmp.DamageDelaySec);
    }

    public static ExplosionFactory FromFile(
        XDocument turretConfig,
        TextureAtlas gameObjectTextures)
    {
        var explosionElements = turretConfig.Root!
            .Element("Explosions")!
            .Elements("Explosion");

        List<ExplosionTemplate> templates = [];

        foreach (var explosionElement in explosionElements)
        {
            var alias = explosionElement.Attribute("alias")?.Value!;
            var animationAlias = explosionElement.Attribute("animationAlias")!.Value;

            var animation = gameObjectTextures.GetAnimation(animationAlias);

            var origin = ParseHelper.ParseToFloatArr(explosionElement, "origin", ';');
            var size = ParseHelper.ParseToFloatArr(explosionElement, "size", ';');
            var scale = TextureHelper.CalculateScale(size, textureRegion: null, animation);

            float damageDelaySec;
            var damageDelayMsValue = explosionElement.Attribute("damageDelayMs")?.Value;
            if (!string.IsNullOrWhiteSpace(damageDelayMsValue))
            {
                var damageDelayMs = float.Parse(damageDelayMsValue);
                damageDelaySec = damageDelayMs / 1000f;
            }
            else
            {
                damageDelaySec = -1;
            }


            templates.Add(
                new ExplosionTemplate(
                    alias,
                    animation,
                    Origin: new Vector2(origin[0], origin[1]),
                    scale,
                    damageDelaySec));
        }

        return new ExplosionFactory(
            templates.ToFrozenDictionary(t => t.Alias));
    }
}