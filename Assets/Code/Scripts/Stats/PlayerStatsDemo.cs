using Unity.Netcode;
using UnityEngine;

public class PlayerStatsDemo : NetStatController
{
    private NetworkVariable<int> gold = new NetworkVariable<int>(0);

    private NetworkVariable<int> currentEXP = new NetworkVariable<int>(
    0,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server
    );

    public void AddGold(int amount)
    {
        if (!IsServer) return;
        gold.Value += amount;
    }

    public void SetGold( int amount)
    {
        if (!IsServer)
            return;
        gold.Value = amount;
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
            currentEXP.Value += amount;
        }
    }

    public void SetEXP(int amount)
    {
        currentEXP.Value = amount;
    }
    public int GetCurrentEXP()
    {
        return currentEXP.Value;
    }
}
