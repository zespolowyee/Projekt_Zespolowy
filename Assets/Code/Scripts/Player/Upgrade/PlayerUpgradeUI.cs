using UnityEngine;
using UnityEngine.UI;

public class PlayerUpgradeUI : MonoBehaviour
{
    [SerializeField] private PlayerUpgradeManager upgradeManager;
    [SerializeField] private Text upgradeDetailsText;
    [SerializeField] private Button upgradeButton;

    private UpgradeNode selectedNode;

    public void DisplayUpgradeDetails(UpgradeNode node)
    {
        selectedNode = node;

        upgradeDetailsText.text = $"{node.description}\n" +
                                  $"Cost: {node.cost} EXP\n" +
                                  $"Effects:\n" +
                                  $"- Health Bonus: {node.effects.healthBonus}\n" +
                                  $"- Damage Bonus: {node.effects.damageBonus}\n" +
                                  $"- Speed Bonus: {node.effects.speedBonus}\n";

        upgradeButton.interactable = !node.isUnlocked;
    }

    public void OnUpgradeButtonClick()
    {
        if (selectedNode != null)
        {
            upgradeManager.UnlockUpgrade(selectedNode.id);
            DisplayUpgradeDetails(selectedNode); // Refresh details
        }
    }
}
