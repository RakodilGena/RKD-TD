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
using RKD_TD.Scenes.Gaming.Flashes;
using RKD_TD.Scenes.Gaming.Projectiles;
using RKD_TD.Scenes.Gaming.Turrets.Purchase;

namespace RKD_TD.Scenes.Gaming.Turrets.Active;

internal sealed class TurretFactory
{
    private readonly FrozenDictionary<TurretType, TurretTemplate> _turrets;
    private readonly ProjectileFactory _projectileFactory;
    private readonly FlashFactory _gunshotFlashFactory;
    private readonly Sprite _turretSelector;

    public TurretFactory(
        FrozenDictionary<TurretType, TurretTemplate> turrets,
        Sprite turretSelector,
        ProjectileFactory projectileFactory,
        FlashFactory gunshotFlashFactory)
    {
        _turrets = turrets;
        _projectileFactory = projectileFactory;
        _gunshotFlashFactory = gunshotFlashFactory;
        _turretSelector = turretSelector;
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
            template,
            destination,
            _turretSelector,
            template.Price,
            template.UpgradePrices,
            _projectileFactory,
            _gunshotFlashFactory);
    }

    public static TurretFactory FromFile(
        XDocument turretConfigDoc,
        TextureAtlas gameObjectTextures,
        out PendingTurretStash pendingTurretStash)
    {
        var turretsElement = turretConfigDoc.Root!.Element("Turrets")!;

        var mgTemplate = CreateTurretTemplate(
            turretsElement,
            gameObjectTextures,
            "MachineGun");

        var cannonTemplate = CreateTurretTemplate(
            turretsElement,
            gameObjectTextures,
            "Cannon");

        var shotgunTemplate = CreateTurretTemplate(
            turretsElement,
            gameObjectTextures,
            "Shotgun");

        var missileTemplate = CreateTurretTemplate(
            turretsElement,
            gameObjectTextures,
            "Missile");

        Dictionary<TurretType, TurretTemplate> templates = new Dictionary<TurretType, TurretTemplate>(4)
        {
            { TurretType.MachineGun, mgTemplate },
            { TurretType.Cannon, cannonTemplate },
            { TurretType.Shotgun, shotgunTemplate },
            { TurretType.Missile, missileTemplate },
        };

        var flashFactory = FlashFactory.FromFile(turretConfigDoc, gameObjectTextures);

        var projectileFactory = ProjectileFactory.FromFile(
            turretConfigDoc,
            gameObjectTextures,
            flashFactory);

        var pendingTurrets = templates.Select(p =>
            new PendingTurret(p.Value.Name, p.Key, p.Value.Price, p.Value.FiringDistance[0]));
        pendingTurretStash = new PendingTurretStash(pendingTurrets);

        var selector = gameObjectTextures.CreateSprite(Textures.Game.SELECTED_TURRET_BORDERS);
        selector.CenterOrigin();

        return new TurretFactory(
            turrets: templates.ToFrozenDictionary(),
            selector,
            projectileFactory,
            flashFactory);
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

        var fixateDistance = ParseHelper.ParseToFloatArr(turretElement, "fixateDistance", ';');
        var firingDistance = ParseHelper.ParseToFloatArr(turretElement, "firingDistance", ';');

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


        var pricesArr = ParseHelper.ParseToIntArr(turretElement, "price", ';');

        var aimingModeValue = turretElement.Attribute("aimingMode")?.Value;
        var aimingMode = aimingModeValue switch
        {
            "predictive" => TurretAimingMode.Predictive,
            _ => TurretAimingMode.StrictEnemy
        };

        var barrelLenghtValue = turretElement.Attribute("barrelLenght")?.Value;
        var barrelLenght = !string.IsNullOrEmpty(barrelLenghtValue)
            ? (int)(int.Parse(barrelLenghtValue) * barrelScale.X)
            : 0;

        var projectileFlightRange = ParseHelper.ParseToFloatArr(turretElement, "projectileFlightRange", ';');
        var directDamage = ParseHelper.ParseToIntArr(turretElement, "directDamage", ';');

        var aoeRangeValue = turretElement.Attribute("aoeRange")?.Value;
        var aoeRange = !string.IsNullOrEmpty(aoeRangeValue)
            ? int.Parse(aoeRangeValue)
            : 0;

        var aoeDamageValue = turretElement.Attribute("aoeDamage")?.Value;
        var aoeDamage = !string.IsNullOrEmpty(aoeDamageValue)
            ? ParseHelper.ParseToIntArr(aoeDamageValue, ';')
            : null;

        var name = turretElement.Attribute("name")!.Value;

        return new TurretTemplate(
            name,
            Price: pricesArr[0],
            UpgradePrices: pricesArr[1..].ToArray(),
            barrelTexture,
            barrelAnimation,
            barrelScale,
            BarrelOrigin: new Vector2(barrelOrigin[0], barrelOrigin[1]),
            carriageTexture,
            carriageScale,
            CarriageOrigin: new Vector2(carriageOrigin[0], carriageOrigin[1]),
            RotationSpeedRadianInSec: MathHelper.ToRadians(rotationSpeed),
            reloadTimeSec,
            FixateDistanceSquared: fixateDistance.Select(fd => fd * fd).ToArray(),
            firingDistance,
            FiringDistanceSquared: firingDistance.Select(fd => fd * fd).ToArray(),
            projectileFlightRange,
            directDamage,
            aoeRange,
            aoeDamage,
            firingPoints,
            firingMode,
            flashPoints,
            aimingMode,
            barrelLenght,
            projectileAlias,
            flashAlias);
    }
}