using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretLevels", menuName = "Scriptable Objects/TurretLevels")]
public class TurretLevels : ScriptableObject
{
    [Tooltip("List of all turret levels with their stats.")]
    public List<TurretLevel> levels;
}

[Serializable]
public struct TurretLevel : INetworkSerializable
{
    [Header("Level Info")]
    [Tooltip("Turret level number.")]
    public int level;

    [Header("Stats")]
    [Tooltip("Damage dealt per shot.")]
    public int damage;
    
    [Tooltip("Time in seconds between each shot.")]
    public float shootingInterval;
    
    [Tooltip("Range of the turret.")]
    public float range;
    
    [Header("Upgrade Info")]
    [Tooltip("Cost to upgrade to this level.")]
    public int upgradeCost;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref level);
        serializer.SerializeValue(ref damage);
        serializer.SerializeValue(ref shootingInterval);
        serializer.SerializeValue(ref range);
        serializer.SerializeValue(ref upgradeCost);
    }
}
