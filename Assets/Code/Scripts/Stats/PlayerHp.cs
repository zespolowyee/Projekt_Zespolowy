using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(PlayerStatsDemo))]
public class PlayerHp : HPSystem
{
    PlayerStatsDemo playerStats;
    private GameObject deathScreenUI;
    private RBController rbController;

    public delegate void HpChangedHandler(ulong clientId, int currentHp, int maxHp);
    public static event HpChangedHandler OnHpChanged;

    public delegate void PlayerDiedHandler(ulong clientId);
    public static event PlayerDiedHandler OnPlayerDied;

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
        deathScreenUI = GameObject.Find("DeathScreen");
        rbController = GetComponent<RBController>();
        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(false);
        }
    }

    public override void TakeDamage(int damage)
    {
        if (IsServer)
        {
            InvokeOnHpChangedClientRpc(currentHP.Value - damage);
            currentHP.Value -= damage;
            if (currentHP.Value <= 0)
            {
                currentHP.Value = 0;
                Die();
            }
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    void InvokeOnHpChangedClientRpc(int currentValue)
    {
        if (OnHpChanged != null)
        {
            OnHpChanged(OwnerClientId, currentValue, (int)playerStats.GetNetStatValue(NetStatType.MaxHp));
        }
    }
    protected override void Die()
    {
        base.Die();
        if (IsServer)
        {
            Debug.Log($"Player {OwnerClientId} died.");
            OnPlayerDied?.Invoke(OwnerClientId);
            DieClientRpc();
        }

    }
    //[Rpc(SendTo.ClientsAndHost)]
    private void DieClientRpc()
    {
        DisablePlayerControls();
        ShowDeathScreen();
    }

    private void DisablePlayerControls()
    {
        if (rbController != null)
        {
            rbController.enabled = false;
        }

        // To do uogólnienia - sword attack nie będzie zawsze na tym obiekcie
        var attack = GetComponent<SwordAttack>();
        if (attack != null)
        {
            attack.enabled = false;
        }
        
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        var capsuleCollider = GetComponent<CapsuleCollider>();
        if (capsuleCollider != null)
        {
            capsuleCollider.enabled = false;
        }
    }

    private void ShowDeathScreen()
    {
        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(true);
        }
    }
    public int GetCurrentHP()
    {
        return currentHP.Value; 
    }

    public int GetMaxHp()
    {
        return (int)playerStats.GetNetStatValue(NetStatType.MaxHp);
    }
}
