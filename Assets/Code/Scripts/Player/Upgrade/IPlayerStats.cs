public interface IPlayerStats
{
    void ApplyUpgrade(UpgradeEffects effects);
    int GetCurrentEXP();
    void DeductEXP(int amount);
}
