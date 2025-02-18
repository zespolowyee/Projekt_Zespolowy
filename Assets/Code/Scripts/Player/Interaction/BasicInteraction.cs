using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class BasicInteraction : NetworkBehaviour, IInteractable
{
    public UnityEvent<GameObject> OnInteraction;
	[SerializeField] private bool shouldInteractionBeSyncedOnNet = true;

	[ServerRpc(RequireOwnership = false)]
	protected virtual void TriggerInteractionServerRpc()
	{
		TriggerInteractionClientRpc();
	}

	[ClientRpc]
	protected virtual void TriggerInteractionClientRpc()
	{
		OnInteraction?.Invoke(null);

	}
	public virtual void Interact(GameObject interactingPlayer)
	{
		if (shouldInteractionBeSyncedOnNet)
		{
			TriggerInteractionServerRpc();
		}
		else
		{
			OnInteraction?.Invoke(interactingPlayer);
		}
	}
}
