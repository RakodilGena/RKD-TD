using System.Collections.Frozen;
using System.Collections.Generic;
using RKD_TD.Scenes.Gaming.Turrets.Active;

namespace RKD_TD.Scenes.Gaming.Turrets.Purchase;

internal sealed class PendingTurretStash
{
    private readonly FrozenDictionary<TurretType, PendingTurret> _turrets;

    public PendingTurret GetPendingTurret(TurretType type)
    {
        return _turrets[type];
    }

    public PendingTurretStash(IEnumerable<PendingTurret> turrets)
    {
        _turrets = turrets.ToFrozenDictionary(t => t.Type);
    }
}