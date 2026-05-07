using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Graphics.Sprites;
using RKD_TD.Scenes.Gaming.PurchaseTurrets;

namespace RKD_TD.Scenes.Gaming.ActiveTurrets;

internal sealed class TurretFactory
{
    private readonly FrozenDictionary<TurretType, TurretTemplate> _turrets;

    public TurretFactory(FrozenDictionary<TurretType, TurretTemplate> turrets)
    {
        _turrets = turrets;
    }

    public Turret CreateTurret(
        BuildCell destination,
        TurretType turretType)
    {
        var template = _turrets[turretType];

        var cellCenter = destination.CellSize * 0.5f;

        var position = destination.WorldPosition + cellCenter;

        var carriageSprite = new Sprite(template.CarriageTexture)
        {
            Scale = template.CarriageScale,
            Origin = template.CarriageOrigin
        };

        Sprite barrelSprite;
        if (template.BarrelTexture != null)
        {
            barrelSprite = new Sprite(template.BarrelTexture);
        }
        else
        {
            barrelSprite = new AnimatedSprite(template.BarrelAnimation!);
        }

        barrelSprite.Scale = template.BarrelScale;
        barrelSprite.Origin = template.BarrelOrigin;

        return new Turret(
            barrelSprite,
            carriageSprite,
            position,
            template.RotationSpeedRadianInSec,
            template.ReloadTimeInSec,
            template.FixateDistanceSquared,
            template.FiringDistanceSquared,
            template.FiringPoints,
            template.ProjectileType,
            destination);
    }

    public static TurretFactory FromFile(
        XDocument doc,
        TextureAtlas gameObjectTextures)
    {
        var turretsElement = doc.Root!.Element("Turrets")!;

        var mgTemplate = CreateTurretTemplate(
            turretsElement,
            gameObjectTextures,
            "MachineGun");

        Dictionary<TurretType, TurretTemplate> templates = new Dictionary<TurretType, TurretTemplate>(4)
        {
            { TurretType.MachineGun, mgTemplate }
        };

        return new TurretFactory(templates.ToFrozenDictionary());
    }

    private static TurretTemplate CreateTurretTemplate(
        XElement turretsElement,
        TextureAtlas gameObjectTextures,
        string turretElementName)
    {
        var turretElement = turretsElement.Element(turretElementName)!;

        var barrelAnimationAlias = turretElement.Attribute("barrelAnimationAlias")?.Value;
        var barrelTextureAlias = turretElement.Attribute("barrelTextureAlias")?.Value;


        if (barrelAnimationAlias is null && barrelTextureAlias is null
            || barrelAnimationAlias is not null && barrelTextureAlias is not null)
            throw new InvalidOperationException("Specify exactly texture or animation.");

        var barrelTexture = string.IsNullOrWhiteSpace(barrelTextureAlias)
            ? null
            : gameObjectTextures.GetRegion(barrelTextureAlias);

        var barrelAnimation = string.IsNullOrWhiteSpace(barrelAnimationAlias)
            ? null
            : gameObjectTextures.GetAnimation(barrelAnimationAlias);

        var barrelOrigin = ParseToFloatArr(turretElement, "barrelOrigin");
        var barrelSize = ParseToFloatArr(turretElement, "barrelSize");
        var barrelScale = CalculateScale(barrelSize, barrelTexture, barrelAnimation);


        var carriageTextureAlias = turretElement.Attribute("carriageTextureAlias")!.Value;
        var carriageTexture = gameObjectTextures.GetRegion(carriageTextureAlias);

        var carriageOrigin = ParseToFloatArr(turretElement, "carriageOrigin");
        var carriageSize = ParseToFloatArr(turretElement, "carriageSize");
        var carriageScale = CalculateScale(carriageSize, carriageTexture, null);

        var rotationSpeed = float.Parse(turretElement.Attribute("rotationSpeed")!.Value);
        var reloadTime = float.Parse(turretElement.Attribute("reloadTime")!.Value);
        var fixateDistance = float.Parse(turretElement.Attribute("fixateDistance")!.Value);
        var firingDistance = float.Parse(turretElement.Attribute("firingDistance")!.Value);
        var projectileType = turretElement.Attribute("projectileType")!.Value;

        var firingPoints = turretElement.Attribute("firingPoints")!.Value
            .Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(point =>
                point.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(float.Parse)
                    .ToArray()
            )
            .Select(pointValues => new TurretFiringPoint(
                Position: new Vector2(pointValues[0], pointValues[1]),
                BulletExtraAngleInRadians: MathHelper.ToRadians(pointValues[2])))
            .ToArray();


        return new TurretTemplate(
            barrelTexture,
            barrelAnimation,
            barrelScale,
            BarrelOrigin: new Vector2(barrelOrigin[0], barrelOrigin[1]),
            carriageTexture,
            carriageScale,
            CarriageOrigin: new Vector2(carriageOrigin[0], carriageOrigin[1]),
            RotationSpeedRadianInSec: MathHelper.ToRadians(rotationSpeed),
            reloadTime,
            FixateDistanceSquared: fixateDistance * fixateDistance,
            FiringDistanceSquared: firingDistance * firingDistance,
            firingPoints,
            projectileType);

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