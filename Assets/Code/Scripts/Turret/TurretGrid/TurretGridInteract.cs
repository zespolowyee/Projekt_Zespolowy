using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class TurretGridInteract : NetworkBehaviour, IInteractableTwoWay
{
    [SerializeField] private bool shouldInteractionBeSyncedOnNet = true; 

    [ServerRpc(RequireOwnership = false)]
    protected virtual void TriggerInteractionServerRpc(TurretManager.TurretType turret)
    {
        TriggerInteractionClientRpc(turret);
    }

    [ClientRpc]
    protected virtual void TriggerInteractionClientRpc(TurretManager.TurretType turret)
    {
        placeTurret(turret);
    }

    public void Interact(Object sender)
    {
        if (sender is PlayerPublicPreferences playerPublicPreferences)
        {
            var turret = playerPublicPreferences.selectedTurret;

            if (shouldInteractionBeSyncedOnNet)
            {
                TriggerInteractionServerRpc(turret);
            }
            else
            {
                placeTurret(turret);
            }
        }
    }
    public void placeTurret(TurretManager.TurretType turret)
    {
        var turretPrefab = TurretManager.GetTurretPrefab(turret);
        if (turretPrefab != null)
        {
            Instantiate(turretPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Turret prefab is null");
        }
    }
}