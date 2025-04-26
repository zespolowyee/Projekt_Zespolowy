using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerClassPrefabMapping", menuName = "Game/Player Class Prefab Mapping")]
public class PlayerClassPrefabMapping : ScriptableObject
{
    [Serializable]
    public struct PlayerClassPrefabEntry
    {
        public PlayerClassType playerClass;
        public GameObject prefab;
    }

    [SerializeField] private List<PlayerClassPrefabEntry> prefabMappings;

    private Dictionary<PlayerClassType, GameObject> _prefabDictionary;

    public void Initialize()
    {
        _prefabDictionary = new Dictionary<PlayerClassType, GameObject>();
        foreach (var entry in prefabMappings)
        {
            if (!_prefabDictionary.ContainsKey(entry.playerClass))
            {
                _prefabDictionary[entry.playerClass] = entry.prefab;
            }
        }
    }

    public GameObject GetPrefab(PlayerClassType playerClass)
    {
        if (_prefabDictionary == null)
        {
            Initialize();
        }

        if (_prefabDictionary.TryGetValue(playerClass, out GameObject prefab))
        {
            return prefab;
        }

        Debug.LogWarning($"Prefab for {playerClass} not found!");
        return null;
    }
}
