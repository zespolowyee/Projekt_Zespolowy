using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HudController : NetworkBehaviour
{
    [SerializeField] private Image health;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI expText;

    [SerializeField] private TextMeshProUGUI currentHpText;

    [SerializeField] private PlayerHp playerHp;
    [SerializeField] private PlayerStatsDemo playerStats;

    private void Start()
    {
        if (!IsOwner)
        {
            Destroy(gameObject);
        }
        else
        {
            PlayerHp.OnHpChanged += UpdateHealthBarData;
            PlayerStatsDemo.OnGoldChanged += UpdateGoldData;
            PlayerStatsDemo.OnExpChanged += UpdateExpData;
            playerStats.GetNetStat(NetStatType.MaxHp).OnModifiersChanged += UpdateHealthBarData;
            if (playerHp != null)
            {
                health.fillAmount = ((float)playerHp.GetCurrentHP() / (float)playerHp.GetMaxHp());
                currentHpText.text = playerHp.currentHP.Value.ToString() + " / " + playerHp.GetMaxHp().ToString();
            }
            if (playerStats != null)
            {
                UpdateGoldData(OwnerClientId, playerStats.GetGold());
                UpdateExpData(OwnerClientId, playerStats.GetCurrentEXP());
            }
        }


    }

    private void OnDisable()
    {
        PlayerHp.OnHpChanged -= UpdateHealthBarData;
        PlayerStatsDemo.OnGoldChanged -= UpdateGoldData;
        PlayerStatsDemo.OnExpChanged -= UpdateExpData;
        playerStats.GetNetStat(NetStatType.MaxHp).OnModifiersChanged -= UpdateHealthBarData;
    }

    public void UpdateHealthBarData(ulong clientId, int currentHp, int maxHp)
    {
        if (currentHp < 0)
        {
            currentHp = 0;
        }
        if (clientId != OwnerClientId)
        {
            return;
        }
        health.fillAmount = ((float)currentHp / (float)maxHp);
        currentHpText.text = currentHp.ToString() + " / " + maxHp;

    }

    public void UpdateHealthBarData()
    {
        health.fillAmount = ((float)playerHp.currentHP.Value / (float)playerHp.GetMaxHp());
        currentHpText.text = playerHp.currentHP.Value.ToString() + " / "+ playerHp.GetMaxHp().ToString();

    }


    public void UpdateGoldData(ulong clientId, int gold)
    {
        if (clientId != OwnerClientId)
        {
            return;
        }
        goldText.text = gold.ToString();

    }

    public void UpdateExpData(ulong clientId, int exp)
    {
        if (clientId != OwnerClientId)
        {
            return;
        }
        expText.text = exp.ToString();

    }

}
