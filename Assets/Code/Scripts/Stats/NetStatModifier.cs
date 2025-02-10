using System;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public struct NetStatModifier : INetworkSerializable
{
    public NetStatType StatType;
    public StatModType ModType; 
    public float Value;
    public int Order;



    public NetStatModifier(StatModType modType, float value, int order, NetStatType type)
    {
        this.ModType = modType;
        this.Value = value;
        this.Order = order;
        this.StatType = type;

    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref StatType);
        serializer.SerializeValue(ref ModType);
        serializer.SerializeValue(ref Value);
        serializer.SerializeValue(ref Order);
    }
}
