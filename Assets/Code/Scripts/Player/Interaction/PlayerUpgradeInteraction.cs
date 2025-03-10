using UnityEngine;

public class PlayerUpgradeInteraction : MonoBehaviour, IInteractable
{
    public void Interact(GameObject interactingPlayer)
    {
        UIController uiController = interactingPlayer.GetComponentInChildren<UIController>();
        PlayerUpgradeManager upgradeManager = uiController.GetComponent<PlayerUpgradeManager>();
        uiController.DisplayPlayerUpgradeUI(upgradeManager.UpgradeTree);
    }

}
