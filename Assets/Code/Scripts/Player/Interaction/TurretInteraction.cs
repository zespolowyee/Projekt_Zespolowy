using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class TurretInteraction : NetworkBehaviour, IInteractable
{
    public UnityEvent<GameObject> OnInteraction;
    
    public virtual void Interact(GameObject interactingPlayer)
    {
        OnInteraction?.Invoke(interactingPlayer);
    }
}