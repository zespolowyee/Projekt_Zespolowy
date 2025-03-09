using Unity.Netcode;
using UnityEngine;

public class PlayerStatsDemo : NetStatController
{
    public  delegate void GoldChangedUpHandler(ulong clientId, int goldAmount);
    public static event GoldChangedUpHandler OnGoldChanged;

    public delegate void ExpChangedHandler(ulong clientId, int expAmount);
    public static event ExpChangedHandler OnExpChanged;

    private NetworkVariable<int> gold = new NetworkVariable<int>(2137);

    private NetworkVariable<int> currentEXP = new NetworkVariable<int>(
    0,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server
    );

    public void AddGold(int amount)
    {
        if (!IsServer)
        {
            return;
        }


        InvokeOnGoldChangedClientRpc(gold.Value + amount);
        gold.Value += amount;
    }

    public void SetGold( int amount)
    {
        if (!IsServer)
        {
            return;
        }
        gold.Value = amount;

        InvokeOnGoldChangedClientRpc(amount);
    }

    public int GetGold()
    {
        return gold.Value; 
    }

    public void AddEXP(int amount)
    {
        if (!IsServer)
        {
            return;
        }

        if (currentEXP.Value + amount < 0)
        {
            Debug.LogError("Not enough exp");
        }
        else
        {


            InvokeOnExpChangedClientRpc(currentEXP.Value + amount);
            currentEXP.Value += amount;
        }
    }

    public void SetEXP(int amount)
    {
        if (!IsServer)
        {
            return;
        }

        currentEXP.Value = amount;
        InvokeOnExpChangedClientRpc(amount);
    }
    public int GetCurrentEXP()
    {
        return currentEXP.Value;
    }

    [Rpc(SendTo.ClientsAndHost)]
    void InvokeOnGoldChangedClientRpc(int currentValue)
    {
        if (OnGoldChanged != null)
        {
            OnGoldChanged(OwnerClientId, currentValue);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    void InvokeOnExpChangedClientRpc(int currentValue)
    {
        if (OnExpChanged != null)
        {
            OnExpChanged(OwnerClientId, currentValue);
        }
    }
}
