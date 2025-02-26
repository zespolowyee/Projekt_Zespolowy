using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class TurretInteraction : NetworkBehaviour, IInteractable
{
    public virtual void Interact(GameObject interactingPlayer)
    {
        UIController uiController = interactingPlayer.GetComponentInChildren<UIController>();
        TurretStats turretStats = gameObject.GetComponentInParent<TurretStats>();
        uiController.DisplayTurretUpgradeUI(turretStats);
    }
}