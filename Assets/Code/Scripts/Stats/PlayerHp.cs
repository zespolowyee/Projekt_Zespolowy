using UnityEngine;

[RequireComponent(typeof(PlayerStatsDemo))]
public class PlayerHp : HPSystem
{
    PlayerStatsDemo playerStats;

    public delegate void HpChangedHandler(ulong clientId, int currentHp);
    public static event HpChangedHandler OnHpChanged;

    protected override void Start()
    {
        if (IsServer)
        {

            if (TryGetComponent<PlayerStatsDemo>(out playerStats))
            {
                currentHP.Value = (int)playerStats.GetNetStatValue(NetStatType.MaxHp);
            }
            else
            {
                currentHP.Value = maxHP;
            }

        }
        animator = GetComponent<Animator>();
    }

    public override void TakeDamage(int damage)
    {
        if (IsServer)
        {
            currentHP.Value -= damage;

            if (OnHpChanged != null)
            {
                OnHpChanged(OwnerClientId, currentHP.Value);
            }

            if (currentHP.Value <= 0)
            {
                currentHP.Value = 0;
                Die();
            }
        }
    }
}
