using System.Collections.Frozen;
using MonoGameLibrary.Cameras;
using RKD_TD.Scenes.Gaming.ActiveTurrets;

namespace RKD_TD.Scenes.Gaming.PurchaseTurrets;

internal static class PendingTurretStash
{
    private static FrozenDictionary<TurretType, PendingTurret> _turrets = null!;

    public static PendingTurret GetPendingTurret(TurretType type)
    {
        return _turrets[type];
    }

    public static void InitPendingTurrets(ICamera camera)
    {
        PendingTurret[] turrets =
        [
            new(TurretType.MachineGun, camera, 100),
            new(TurretType.Cannon, camera, 200),
            new(TurretType.Shotgun, camera, 250),
            new(TurretType.Rocket, camera, 300)
        ];

        _turrets = turrets.ToFrozenDictionary(t => t.Type);
    }
}