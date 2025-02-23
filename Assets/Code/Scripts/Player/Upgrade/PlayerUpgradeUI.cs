using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PlayerUpgradeUI : MonoBehaviour
{
    [SerializeField] private PlayerUpgradeManager upgradeManager;
    [SerializeField] private PlayerUpgradeTree upgradeTree;
    [SerializeField] private TMP_Text upgradeDetailsText;
    [SerializeField] private Button upgradeButton;

    //[SerializeField] private Transform buttonContainer;
    //[SerializeField] private TMP_Text expText;


    private UpgradeNode selectedNode;


    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonParent; // Parent UI object to hold buttons


    private void Start()
    {
        CreateUpgradeButtons();
    }
    public void CreateUpgradeButtons()
    {
        // Clear existing buttons before generating new ones
        foreach (Transform child in buttonParent)
        {
            Destroy(child.gameObject);
        }

        // Create a button for each upgrade node
        foreach (var node in upgradeTree.nodes)
        {
            // Instantiate a new button from a prefab
            GameObject newButton = Instantiate(buttonPrefab, buttonParent);

            // Set the text of the button to the node's description
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = node.description;

            var localNode = node;
            // Add listener for the button click to select this node
            newButton.GetComponent<Button>().onClick.AddListener(() => SelectUpgradeNode(localNode));
            newButton.GetComponent<Button>().onClick.AddListener(() => OnUpgradeButtonClick());


            // Set the position of the button based on the node's position
            //RectTransform rectTransform = newButton.GetComponent<RectTransform>();
            //rectTransform.anchoredPosition = new Vector2(node.positionX, node.positionY);
        }
    }

    public void SelectUpgradeNode(UpgradeNode node)
    {
        // Set the selected node
        selectedNode = node;

        // Optionally, display the upgrade details in the UI
        DisplayUpgradeDetails(node);
    }
    public void DisplayUpgradeDetails(UpgradeNode node)
    {
        if (node == null) return;

        selectedNode = node;
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
