using System;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public struct NetStatModifier : INetworkSerializable
{
    public NetStatType StatType;
    public StatModType ModType; 
    public float Value;



    public NetStatModifier(StatModType modType, float value, NetStatType type)
    {
        this.ModType = modType;
        this.Value = value;
        this.StatType = type;

    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref StatType);
        serializer.SerializeValue(ref ModType);
        serializer.SerializeValue(ref Value);
    }
}
