using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetStatController : NetworkBehaviour
{
    [SerializeField] private List<NetStat> statList;

    [ServerRpc(RequireOwnership = true)]
    public void AddModifierServerRPC(NetStatModifier modifier)
    {
        AddModifierClientRPC(modifier);
    }

    [ServerRpc(RequireOwnership = true)]
    public void RemoveModifierServerRPC(NetStatModifier modifier)
    {
        RemoveModifierClientRPC(modifier);
    }

    [ClientRpc]
    public void RemoveModifierClientRPC(NetStatModifier modifier)
    {

        foreach (NetStat stat in statList)
        {
            if (stat.Type == modifier.StatType)
            {
                stat.RemoveModifier(modifier);
            }
        }
    }

    [ClientRpc]
    public void AddModifierClientRPC(NetStatModifier modifier)
    {

        Debug.Log(modifier.Value);
        foreach (NetStat stat in statList)
        {
            if (stat.Type == modifier.StatType)
            {
                stat.AddModifier(modifier);
            }
        }
    }

    public float GetNetStatValue(NetStatType type)
    {
        foreach (NetStat stat in statList)
        {
            Debug.Log("lelele");
            if (type == stat.Type)
            {
                Debug.Log("");
                return stat.Value;
            }
        }


        Debug.LogError("not found stat of type:" + type);
        return 0f;
    }

}
