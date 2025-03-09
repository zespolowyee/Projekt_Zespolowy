using UnityEngine;

public class PlayerUpgradeInteraction : MonoBehaviour, IInteractable
{
    public void Interact(GameObject interactingPlayer)
    {
        UIController uiController = interactingPlayer.GetComponentInChildren<UIController>();
        uiController.DisplayPlayerUpgradeUI();
    }

}
