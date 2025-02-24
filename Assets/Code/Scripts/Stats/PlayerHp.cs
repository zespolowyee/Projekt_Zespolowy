using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(PlayerStatsDemo))]
public class PlayerHp : HPSystem
{
    PlayerStatsDemo playerStats;

    public delegate void HpChangedHandler(ulong clientId, int currentHp, int maxHp);
    public static event HpChangedHandler OnHpChanged;

    protected override void Start()
    {

        if (TryGetComponent<PlayerStatsDemo>(out playerStats))
        {
            if (IsServer)
            {
                maxHP = (int)playerStats.GetNetStatValue(NetStatType.MaxHp);
                currentHP.Value = maxHP;
            }
        }
        else
        {
            Debug.LogError("player does not have maxHp statistic");
        }


        animator = GetComponent<Animator>();
    }

    public override void TakeDamage(int damage)
    {
        if (IsServer)
        {
            currentHP.Value -= damage;

            InvokeOnHpChangedClientRpc();

            if (currentHP.Value <= 0)
            {
                currentHP.Value = 0;
                Die();
            }
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    void InvokeOnHpChangedClientRpc()
    {
        if (OnHpChanged != null)
        {
            OnHpChanged(OwnerClientId, currentHP.Value, (int)playerStats.GetNetStatValue(NetStatType.MaxHp));
        }
    }
}
