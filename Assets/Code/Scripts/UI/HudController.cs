using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HudController : NetworkBehaviour
{
    [SerializeField] private Image health;

    private void Start()
    {
        if (!IsOwner)
        {
            Destroy(gameObject);
        }
        else
        {
            PlayerHp.OnHpChanged += UpdateHealthBarData;
        }


    }

    private void OnDisable()
    {
        PlayerHp.OnHpChanged -= UpdateHealthBarData;
    }

    public void UpdateHealthBarData(ulong clientId, int currentHp, int maxHp)
    {
        if (clientId != OwnerClientId)
        {
            return;
        }
        health.fillAmount = ((float)currentHp / (float)maxHp);

    }

}
