using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class TurretGridInteract : NetworkBehaviour, IInteractable
{
    [SerializeField] private bool shouldInteractionBeSyncedOnNet = true;
    [SerializeField] private GameObject turretPrefab;
    [ServerRpc(RequireOwnership = false)]
    protected virtual void TriggerInteractionServerRpc()
    {
        TriggerInteractionClientRpc();
    }

    [ClientRpc]
    protected virtual void TriggerInteractionClientRpc()
    {
        placeTurret();
        Destroy(gameObject); // Destroy the grid after placing the turret
    }

    public void Interact(Object sender)
    {
        if (sender is PlayerPublicPreferences playerPublicPreferences)
        {

            if (shouldInteractionBeSyncedOnNet)
            {
                TriggerInteractionServerRpc();
            }
            else
            {
                placeTurret();

            }
        }
    }
    public void placeTurret()
    {
        if (turretPrefab != null)
        {
            Instantiate(turretPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Turret prefab is null");
        }
    }


    void IInteractable.Interact(GameObject interactingPlayer)
    {
        TriggerInteractionServerRpc();
    }
}