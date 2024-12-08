using System.IO;
using Unity.IO.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;


public class BasicLookReaction : NetworkBehaviour, ILookable
{
	[SerializeField] private bool isLookedAtEventOneShot = true;
	[SerializeField] private bool shouldLookingBeSyncedOnNet = true;

	public UnityEvent OnBeingLookedAt;
	public UnityEvent OnLookingAway;

	private bool wasLookTriggered = false;
	private bool isCurrentlyLookedAt = false;
	private bool wasLookedAtLastFrame = false;

	[ServerRpc(RequireOwnership = false)]
	private void TriggerLookedAtServerRpc()
	{
		TriggerLookedAtClientRpc();
	}

	[ServerRpc(RequireOwnership = false)]
	private void TriggerLookAwayServerRpc()
	{
		TriggerLookAwayClientRpc();
	}

	[ClientRpc]
	private void TriggerLookedAtClientRpc()
	{
		OnBeingLookedAt?.Invoke();

	}

	[ClientRpc]
	private void TriggerLookAwayClientRpc()
	{
		OnLookingAway?.Invoke();
	}


	public void DoWhenLookAway()
	{
		OnLookingAway?.Invoke();
		if (shouldLookingBeSyncedOnNet)
		{
			TriggerLookAwayServerRpc();
		}
		wasLookTriggered = false;
	}

	public void DoWhenLookedAt()
	{
		isCurrentlyLookedAt = true;
		if (isLookedAtEventOneShot) {

			if (!wasLookTriggered)
			{
				OnBeingLookedAt?.Invoke();
				if (shouldLookingBeSyncedOnNet)
				{
					TriggerLookedAtServerRpc();
				}
				wasLookTriggered = true;
			}
		}
		else
		{
			OnBeingLookedAt?.Invoke();
			if (shouldLookingBeSyncedOnNet)
			{
				TriggerLookedAtServerRpc();
			}
		}
	}



	private void Update()
	{
		if(wasLookedAtLastFrame && !isCurrentlyLookedAt)
		{
			if (!isCurrentlyLookedAt)
			{
				DoWhenLookAway();
			}
		}

		wasLookedAtLastFrame = isCurrentlyLookedAt;
		isCurrentlyLookedAt = false;
	}
}
