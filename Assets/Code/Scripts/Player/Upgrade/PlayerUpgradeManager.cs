using UnityEngine;

public class PlayerUpgradeManager : MonoBehaviour
{
    [Header("Player Upgrade Tree")]
    [SerializeField] private PlayerUpgradeTree upgradeTree;

    [Header("Player Reference")]
    [SerializeField] private PlayerStats playerStats;


    private void Start()
    {
        foreach (var node in upgradeTree.nodes)
        {
            if (!node.isUnlocked)
            {
                node.isUnlocked = false;
                Debug.Log($"Node {node.id} ({node.description}) set to isUnlocked = false.");
            }
        }
    }


    public void UnlockUpgrade(string nodeId)
    {
        var node = upgradeTree.nodes.Find(n => n.id == nodeId);

        if (node == null)
        {
            Debug.LogError($"Upgrade node with ID {nodeId} not found!");
            return;
        }

        Debug.Log($"Attempting to unlock: {node.description}, Cost: {node.cost} EXP");


        if (node.isUnlocked)
        {
            Debug.Log("Upgrade already unlocked!");
            return;
        }

        if (playerStats.GetCurrentEXP() < node.cost)
        {
            Debug.Log("Not enough EXP to unlock this upgrade.");
            return;
        }

        // Deduct EXP and unlock upgrade
        playerStats.DeductEXP(node.cost);
        node.isUnlocked = true;
        playerStats.ApplyUpgrade(node.effects);

        Debug.Log($"Upgrade {node.description} unlocked!");

        // Optionally display child nodes
        DisplayChildNodes(node);
    }

    private void DisplayChildNodes(UpgradeNode node)
    {
        Debug.Log("Next possible upgrades:");
        foreach (var childId in node.childNodes)
        {
            var childNode = upgradeTree.nodes.Find(n => n.id == childId);
            if (childNode != null && !childNode.isUnlocked)
            {
                Debug.Log($"- {childNode.description} (Cost: {childNode.cost} EXP)");
            }
        }
    }

    


}
