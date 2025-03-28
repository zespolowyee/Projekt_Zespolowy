using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUpgradeDetailsUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI upgreadeNameField;
    [SerializeField] TextMeshProUGUI upgreadeCostField;
    [SerializeField] TextMeshProUGUI upgreadeDescriptionField;
    [SerializeField] TextMeshProUGUI upgreadeEffectsField;
    [SerializeField] Button buyButton;

    private UpgradeNode node;

    private PlayerUpgradeManager upgradeManager;

    public PlayerUpgradeManager UpgradeManager { get => upgradeManager; set => upgradeManager = value; }

    public void DisplayNodeInfo(UpgradeNode node)
    {
        if(node == null)
        {
            return;
        }
        this.node = node;
        upgreadeNameField.text = node.id;
        upgreadeDescriptionField.text = node.description;
        upgreadeCostField.text = $"Cost: {node.cost}"; 

        StringBuilder sb = new StringBuilder();
        foreach(NetStatModifier modifier in node.effects.modifiers)
        {
            sb.Append(modifier.GetEffectString());
            sb.Append(" \n");
        }
        upgreadeEffectsField.text = sb.ToString();

    }

    public void BuyUpgrade()
    {
        upgradeManager.UnlockUpgrade(node.id);
    }

}
