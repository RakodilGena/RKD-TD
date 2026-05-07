using System.Collections.Frozen;
using MonoGameLibrary.Cameras;

namespace RKD_TD.Scenes.Gaming.PurchaseTurrets;

internal static class PendingTurretStash
{
    private static FrozenDictionary<PendingTurretType, PendingTurret> _turrets = null!;

    public static PendingTurret GetPendingTurret(PendingTurretType type)
    {
        return _turrets[type];
    }

    public static void InitPendingTurrets(ICamera camera)
    {
        PendingTurret[] turrets =
        [
            new(PendingTurretType.MachineGun, camera, 100),
            new(PendingTurretType.Cannon, camera, 200),
            new(PendingTurretType.Shotgun, camera, 250),
            new(PendingTurretType.Rocket, camera, 300)
        ];

        _turrets = turrets.ToFrozenDictionary(t => t.Type);
    }
}