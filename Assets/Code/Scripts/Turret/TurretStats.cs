using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class TurretStats : NetStatController
{
    public UnityEvent OnLevelChanged;

    [SerializeField] private TurretLevels turretLevels;
    private GameObject upgradeUIObject;
    private NetworkVariable<int> currentLevelIndex = new NetworkVariable<int>(0);
    private int maxLevelIndex;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
            AddLevelModifiers(GetCurrentLevel());

        currentLevelIndex.OnValueChanged += HandleLevelChanged;
    }

    private void Awake()
    {
        maxLevelIndex = turretLevels.levels.Count - 1;
        OnLevelChanged = new UnityEvent();
    }

    public Boolean NextLevelExists()
    {
        return currentLevelIndex.Value < maxLevelIndex;
    }

    public Boolean HasEnoughGold(GameObject player)
    {
        return GetNextLevel().upgradeCost <= player.GetComponent<PlayerStatsDemo>().GetGold();
    }

    private void AddLevelModifiers(TurretLevel level)
    {
        foreach (NetStatModifier modifier in level.modifiers)
        {
            AddModifierServerRPC(modifier);
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void UpgradeServerRPC(ulong playerObjectId)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerObjectId,
                out NetworkObject playerNetworkObject))
            return;
        
        GameObject player = playerNetworkObject.gameObject;
        player.GetComponentInChildren<PlayerStatsDemo>().AddGold(-GetNextLevel().upgradeCost);
        
        AddLevelModifiers(GetNextLevel());
        currentLevelIndex.Value++;
    }
    
    public bool Upgrade(GameObject upgradingPlayer)
    {
        if (!NextLevelExists())
            return false;
        
        if (!HasEnoughGold(upgradingPlayer))
            return false;
        
        ulong playerId = upgradingPlayer.GetComponentInParent<NetworkObject>().NetworkObjectId;
        
        UpgradeServerRPC(playerId);
        return true;
    }

    public TurretLevel GetCurrentLevel()
    {
        return turretLevels.levels[currentLevelIndex.Value];
    }
    
    public TurretLevel GetNextLevel()
    {
        return turretLevels.levels[currentLevelIndex.Value+1];
    }

    private void HandleLevelChanged(int oldValue, int newValue)
    {
        OnLevelChanged.Invoke();
    }
}
