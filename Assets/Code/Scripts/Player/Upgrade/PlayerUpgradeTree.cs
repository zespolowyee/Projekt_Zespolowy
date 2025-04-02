using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerUpgradeTree", menuName = "Scriptable Objects/Player Upgrade Tree")]
public class PlayerUpgradeTree : ScriptableObject
{
    [Tooltip("List of all upgrade nodes in the tree.")]
    public List<UpgradeNode> nodes;
}

[Serializable]
public class UpgradeNode
{
    [Header("Upgrade Info")]
    [Tooltip("Unique ID for this node.")]
    public string id;

    [Tooltip("Description of the upgrade.")]
    public string description;

    [Tooltip("Cost in EXP to unlock this upgrade.")]
    public int cost;

    [Tooltip("Stat changes provided by this upgrade.")]
    public UpgradeEffects effects;

    [Tooltip("IDs of child nodes that can be unlocked after this.")]
    public List<string> childNodes;

    [Header("State")]
    [Tooltip("Is this upgrade unlocked?")]
    public bool isUnlocked;

    [Tooltip("Is this upgrade unlocked?")]
    public bool isRoot;
}

[Serializable]
public class UpgradeEffects
{
    [Tooltip("List of stat modifiers applied by this upgrade.")]
    public List<NetStatModifier> modifiers;
}
