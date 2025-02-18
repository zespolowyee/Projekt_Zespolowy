using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIScript : MonoBehaviour
{
    public TMP_Text LevelValue;
    public TMP_Text DamageValue;
    public TMP_Text RangeValue;
    public TMP_Text ShootingIntervalValue;
    
    public TurretStats turretStats { set; get; }
    public GameObject player { set; get; }

    public void CloseWindow()
    {
        player.GetComponentInChildren<RBController>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Destroy(gameObject);
    }
    
    public void Upgrade()
    {
        turretStats.UpgradeServerRPC();
    }

    public void UpdateUi()
    {
        LevelValue.text = turretStats.GetCurrentLevel().level.ToString();
        DamageValue.text = turretStats.GetNetStatValue(NetStatType.Damage).ToString();
        RangeValue.text = turretStats.GetNetStatValue(NetStatType.Range).ToString();
        ShootingIntervalValue.text = turretStats.GetNetStatValue(NetStatType.ShootingInterval).ToString();
    }

}
