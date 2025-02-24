using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetStatController : NetworkBehaviour, INetStatController
{
    [SerializeField] private List<NetStat> statList;

    [Rpc(SendTo.Server)]
    public virtual void AddModifierServerRPC(NetStatModifier modifier)
    {
        AddModifierClientRPC(modifier);
    }

    [Rpc(SendTo.Server)]
    public virtual void RemoveModifierServerRPC(NetStatModifier modifier)
    {
        RemoveModifierClientRPC(modifier);
    }

    [Rpc(SendTo.ClientsAndHost)]
    protected virtual void RemoveModifierClientRPC(NetStatModifier modifier)
    {

        foreach (NetStat stat in statList)
        {
            if (stat.Type == modifier.StatType)
            {
                stat.RemoveModifier(modifier);
                return;
            }
        }
        Debug.LogError("No stat of type" + modifier.StatType + " in controller");
    }

    [Rpc(SendTo.ClientsAndHost)]
    protected virtual void AddModifierClientRPC(NetStatModifier modifier)
    {
        foreach (NetStat stat in statList)
        {
            if (stat.Type == modifier.StatType)
            {
                stat.AddModifier(modifier);
                return;
            }
        }
        Debug.LogError("No stat of type" + modifier.StatType + " in controller");
    }

    public float GetNetStatValue(NetStatType type)
    {
        foreach (NetStat stat in statList)
        {
            if (type == stat.Type)
            {
                return stat.Value;
            }
        }
        Debug.LogError("No stat of type" + type + " in controller");
        return 0f;
    }

}
