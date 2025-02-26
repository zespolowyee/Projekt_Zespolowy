using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurretUpgradeUIWindow : Window
{
    public TMP_Text LevelValue;
    public TMP_Text DamageValue;
    public TMP_Text RangeValue;
    public TMP_Text ShootingIntervalValue;
    public TMP_Text UpgradeCostValue;
    public Button UpgradeButton;
    
    public TurretStats turretStats { set; get; }

    public void Start()
    {
        UpdateUi();
        turretStats.OnLevelChanged.AddListener(UpdateUi);
    }

    public void Upgrade()
    {
        var wasUpgraded = turretStats.Upgrade(player);
        if (!wasUpgraded)
            UpdateUi();
    }

    public bool CanUpgrade()
    {
        int playersGold = player.GetComponent<PlayerStatsDemo>().GetGold();
        if (!turretStats.NextLevelExists())
            return false;
        return playersGold >= turretStats.GetNextLevel().upgradeCost;
    }

    public void UpdateUi()
    {
        UpgradeButton.interactable = CanUpgrade();

        if (turretStats.NextLevelExists())
        {
            UpgradeCostValue.text = turretStats.GetNextLevel().upgradeCost.ToString();
        }
        else
        {
            UpgradeCostValue.text = "-";
        }

        LevelValue.text = turretStats.GetCurrentLevel().level.ToString();
        DamageValue.text = turretStats.GetNetStatValue(NetStatType.Damage).ToString();
        RangeValue.text = turretStats.GetNetStatValue(NetStatType.Range).ToString();
        ShootingIntervalValue.text = turretStats.GetNetStatValue(NetStatType.ShootingInterval).ToString();
    }
}
