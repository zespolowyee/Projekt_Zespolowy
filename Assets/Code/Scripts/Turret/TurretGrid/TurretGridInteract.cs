using Unity.Netcode;
using UnityEngine;


public class TurretGridInteract : NetworkBehaviour, IInteractableTwoWay
{
    public void Interact(Object sender)
    {
        if (sender is PlayerPublicPreferences playerPublicPreferences)
        {
            Instantiate(playerPublicPreferences.selectedTurret, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Sender is not of type PlayerPublicPreferences");
        }
    }
}