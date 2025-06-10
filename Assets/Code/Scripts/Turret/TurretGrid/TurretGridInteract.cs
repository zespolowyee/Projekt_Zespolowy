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
                RequestTurretPlacementServerRpc(transform.position);

            }
        }
    }
    public void placeTurret()
    {
        if (turretPrefab != null)
        {
            // Sprawdü czy mamy uprawnienia do spawnowania (tylko serwer/host)
            if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
            {
                GameObject turretInstance = Instantiate(turretPrefab, transform.position, Quaternion.identity);
                NetworkObject turretNetworkObject = turretInstance.GetComponent<NetworkObject>();

                if (turretNetworkObject != null)
                {
                    turretNetworkObject.Spawn();
                }
                else
                {
                    Debug.LogError("Turret prefab doesn't have NetworkObject component!");
                    Destroy(turretInstance);
                }
            }
            else
            {
                // Jeúli to klient, wyúlij request do serwera
                RequestTurretPlacementServerRpc(transform.position);
            }
        }
        else
        {
            Debug.LogError("Turret prefab is null");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestTurretPlacementServerRpc(Vector3 position)
    {
        GameObject turretInstance = Instantiate(turretPrefab, position, Quaternion.identity);
        NetworkObject turretNetworkObject = turretInstance.GetComponent<NetworkObject>();

        if (turretNetworkObject != null)
        {
            turretNetworkObject.Spawn();
        }
    }


    void IInteractable.Interact(GameObject interactingPlayer)
    {
        TriggerInteractionServerRpc();
    }
}