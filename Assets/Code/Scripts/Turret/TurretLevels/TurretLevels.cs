using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretLevels", menuName = "Scriptable Objects/TurretLevels")]
public class TurretLevels : ScriptableObject
{
    [SerializeField]
    private List<NetStatType> usedTypes;
    
    [Tooltip("List of all turret levels with their modifiers.")]
    public List<TurretLevel> levels;
    
    private void OnValidate()
    {
        for (int i = 0; i < levels.Count; i++)
        {
            var level = levels[i];
            
            level.level = i+1;

            foreach (NetStatType stat in usedTypes)
            {
                if (!level.modifiers.Exists(m => m.StatType == stat))
                {
                    level.modifiers.Add(new NetStatModifier { StatType = stat, ModType = StatModType.Flat, Value = 0});
                }
            }

            levels[i] = level;
        }
    }
}

[Serializable]
public struct TurretLevel
{
    [Header("Level Info")]
    [Tooltip("Turret level number.")]
    public int level;

    [Header("Modifiers")]
    [Tooltip("List of modifiers.")]
    public List<NetStatModifier> modifiers;
    
    [Header("Upgrade Info")]
    [Tooltip("Cost to upgrade to this level.")]
    public int upgradeCost;
}
