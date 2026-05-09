using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Graphics.Sprites;
using RKD_TD.Helpers;
using RKD_TD.Scenes.Gaming.Flashes;
using RKD_TD.Scenes.Gaming.Projectiles;
using RKD_TD.Scenes.Gaming.PurchaseTurrets;

namespace RKD_TD.Scenes.Gaming.ActiveTurrets;

internal sealed class TurretFactory
{
    private readonly FrozenDictionary<TurretType, TurretTemplate> _turrets;
    private readonly ProjectileFactory _projectileFactory;
    private readonly FlashFactory _gunshotFlashFactory;

    public TurretFactory(
        FrozenDictionary<TurretType, TurretTemplate> turrets,
        ProjectileFactory projectileFactory,
        FlashFactory gunshotFlashFactory)
    {
        _turrets = turrets;
        _projectileFactory = projectileFactory;
        _gunshotFlashFactory = gunshotFlashFactory;
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
            template.FiringMode,
            template.GunFlashPoints,
            template.ProjectileAlias,
            template.FlashAlias,
            destination,
            _projectileFactory,
            _gunshotFlashFactory);
    }

    public static TurretFactory FromFile(
        XDocument turretConfigDoc,
        TextureAtlas gameObjectTextures)
    {
        var turretsElement = turretConfigDoc.Root!.Element("Turrets")!;

        var mgTemplate = CreateTurretTemplate(
            turretsElement,
            gameObjectTextures,
            "MachineGun");

        Dictionary<TurretType, TurretTemplate> templates = new Dictionary<TurretType, TurretTemplate>(4)
        {
            { TurretType.MachineGun, mgTemplate }
        };

        var projectileFactory = ProjectileFactory.FromFile(turretConfigDoc, gameObjectTextures);
        var gunShotFlashFactory = FlashFactory.FromFile(turretConfigDoc, gameObjectTextures);

        return new TurretFactory(
            turrets: templates.ToFrozenDictionary(),
            projectileFactory,
            gunShotFlashFactory);
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

        var barrelOrigin = ParseHelper.ParseToFloatArr(turretElement, "barrelOrigin", ';');
        var barrelSize = ParseHelper.ParseToFloatArr(turretElement, "barrelSize", ';');
        var barrelScale = TextureHelper.CalculateScale(barrelSize, barrelTexture, barrelAnimation);


        var carriageTextureAlias = turretElement.Attribute("carriageTextureAlias")!.Value;
        var carriageTexture = gameObjectTextures.GetRegion(carriageTextureAlias);

        var carriageOrigin = ParseHelper.ParseToFloatArr(turretElement, "carriageOrigin", ';');
        var carriageSize = ParseHelper.ParseToFloatArr(turretElement, "carriageSize", ';');
        var carriageScale = TextureHelper.CalculateScale(carriageSize, carriageTexture, null);

        var rotationSpeed = float.Parse(turretElement.Attribute("rotationSpeed")!.Value);
        var reloadTimeMs = float.Parse(turretElement.Attribute("reloadTimeMs")!.Value);
        var reloadTimeSec = reloadTimeMs / 1000f;
        var fixateDistance = float.Parse(turretElement.Attribute("fixateDistance")!.Value);
        var firingDistance = float.Parse(turretElement.Attribute("firingDistance")!.Value);
        var projectileAlias = turretElement.Attribute("projectileAlias")!.Value;
        var flashAlias = turretElement.Attribute("flashAlias")!.Value;

        var firingPoints = turretElement.Attribute("firingPoints")!.Value
            .Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(point => ParseHelper.ParseToFloatArr(point, ','))
            .Select(pointValues => new TurretFiringPoint(
                Position: new Vector2(
                    x: pointValues[0] * barrelScale.X,
                    y: pointValues[1] * barrelScale.Y),
                BulletExtraAngleInRadians: MathHelper.ToRadians(pointValues[2])))
            .ToArray();

        var firingModeValue = turretElement.Attribute("firingMode")?.Value;
        var firingMode = firingModeValue switch
        {
            "random" => TurretFiringMode.Random,
            "all" => TurretFiringMode.All,
            "rotation" => TurretFiringMode.Rotation,
            _ => TurretFiringMode.Single
        };

        var flashPoints = turretElement.Attribute("flashPoints")!.Value
            .Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(point => ParseHelper.ParseToFloatArr(point, ','))
            .Select(pointValues => new Vector2(
                x: pointValues[0] * barrelScale.X,
                y: pointValues[1] * barrelScale.Y))
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
            reloadTimeSec,
            FixateDistanceSquared: fixateDistance * fixateDistance,
            FiringDistanceSquared: firingDistance * firingDistance,
            firingPoints,
            firingMode,
            flashPoints,
            projectileAlias,
            flashAlias);
    }
}