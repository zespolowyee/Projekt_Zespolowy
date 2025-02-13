using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : NetStatController
{
   
    [Header("Experience Points")]
    [SerializeField] private int currentEXP = 1000;

    public Dictionary<NetStatType, float> stats = new Dictionary<NetStatType, float>();
    [SerializeField] private PlayerUpgradeTree upgradeTree;


    public void ApplyUpgrade(UpgradeEffects effects)
    {
        foreach (var modifier in effects.modifiers)
        {
            AddModifierServerRPC(modifier);
        }
        
    }

    public int GetCurrentEXP()
    {
        return currentEXP;
    }

    public void DeductEXP(int amount)
    {
        if (amount > currentEXP)
        {
            Debug.LogError("Not enough EXP!");
            return;
        }

        currentEXP -= amount;
        Debug.Log($"EXP deducted: {amount}. Remaining EXP: {currentEXP}");
    }

    public void AddEXP(int amount)
    {
        currentEXP += amount;
        Debug.Log($"EXP added: {amount}. Current EXP: {currentEXP}");
    }

    public void PurchaseUpgrade(UpgradeNode node)
    {
        if (GetCurrentEXP() >= node.cost && !node.isUnlocked)
        {
            DeductEXP(node.cost);
            ApplyUpgrade(node.effects);
            node.isUnlocked = true;
        }
        else
        {
            Debug.LogError("Not enough EXP or upgrade already purchased!");
        }
    }

}
