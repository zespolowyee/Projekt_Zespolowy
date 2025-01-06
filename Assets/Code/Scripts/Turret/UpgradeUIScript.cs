using TMPro;
using UnityEngine;

public class UpgradeUIScript : MonoBehaviour
{
    public TMP_Text LevelValue;
    public TMP_Text DamageValue;
    public TMP_Text RangeValue;
    public TMP_Text ShootingIntervalValue;

    public void DisplayNewLevel(TurretLevel turretLevel)
    {
        LevelValue.text = turretLevel.level.ToString();
        DamageValue.text = turretLevel.damage.ToString();
        RangeValue.text = turretLevel.range.ToString();
        ShootingIntervalValue.text = turretLevel.shootingInterval.ToString();
    }

}
