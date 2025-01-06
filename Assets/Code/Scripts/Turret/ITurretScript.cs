using Unity.Netcode;
using UnityEngine;

public interface ITurretScript
{
    [ServerRpc]
    public void SetTurretLevelServerRpc(TurretLevel turretLevel);
}
