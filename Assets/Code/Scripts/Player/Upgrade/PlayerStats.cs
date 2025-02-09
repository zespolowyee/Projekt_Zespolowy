using UnityEngine;

public class PlayerStats : MonoBehaviour, IPlayerStats
{
    [Header("Player Stats")]
    [SerializeField] private int health = 100;
    [SerializeField] private int damage = 10;
    [SerializeField] private float speed = 5f;

    [Header("Experience Points")]
    [SerializeField] private int currentEXP = 1000;

    public void ApplyUpgrade(UpgradeEffects effects)
    {
        health += effects.healthBonus;
        damage += effects.damageBonus;
        speed += effects.speedBonus;

        Debug.Log($"Upgrade applied! New stats: Health={health}, Damage={damage}, Speed={speed}");
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
        Debug.Log($"Gained {amount} EXP! Total EXP: {currentEXP}");
    }
}
