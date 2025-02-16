using Unity.Netcode;
using UnityEngine;

public interface INetStatController
{

    public void AddModifierServerRPC(NetStatModifier modifier);

    public void RemoveModifierServerRPC(NetStatModifier modifier);

    public float GetNetStatValue(NetStatType type);
}
