using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerUpgradeUI : MonoBehaviour
{
    [SerializeField] private PlayerUpgradeManager upgradeManager;
    [SerializeField] private PlayerUpgradeTree upgradeTree;
    [SerializeField] private Text upgradeDetailsText;
    [SerializeField] private Button upgradeButton;

    private UpgradeNode selectedNode;

    public void DisplayUpgradeDetails(UpgradeNode node)
    {
        string effectsText = "Effects:\n";
        foreach (var modifier in node.effects.modifiers)
        {
            effectsText += $"- {modifier.StatType}: {modifier.Value} ({modifier.ModType})\n";
        }

        upgradeDetailsText.text = $"{node.description}\n" +
                                  $"Cost: {node.cost} EXP\n" +
                                  effectsText;

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
