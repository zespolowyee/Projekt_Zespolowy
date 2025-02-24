using System;
using Unity.Netcode;
using UnityEngine;

public class TurretStats : NetStatController
{
    [SerializeField] private GameObject upgradeUI;
    
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
        
        TurretInteraction[] interactions = GetComponentsInChildren<TurretInteraction>();

        foreach (var interaction in interactions)
        {
            interaction.OnInteraction.AddListener(DisplayUpgradeUI);
        }
    }

    public Boolean CanUpgrade()
    {
        return currentLevelIndex.Value < maxLevelIndex;
    }

    private void AddLevelModifiers(TurretLevel level)
    {
        foreach (NetStatModifier modifier in level.modifiers)
        {
            AddModifierServerRPC(modifier);
        }
    }
    
    private void RemoveLevelModifiers(TurretLevel level)
    {
        foreach (NetStatModifier modifier in level.modifiers)
        {
            RemoveModifierServerRPC(modifier);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpgradeServerRPC()
    {
        if (!CanUpgrade())
            return;
            
        RemoveLevelModifiers(GetCurrentLevel());
        AddLevelModifiers(GetNextLevel());
        currentLevelIndex.Value++;
    }

    public TurretLevel GetCurrentLevel()
    {
        return turretLevels.levels[currentLevelIndex.Value];
    }
    
    public TurretLevel GetNextLevel()
    {
        return turretLevels.levels[currentLevelIndex.Value+1];
    }

    public void DisplayUpgradeUI(GameObject interactingPlayer)
    {
        if(upgradeUIObject == null)
            upgradeUIObject = Instantiate(upgradeUI);
            upgradeUIObject.GetComponent<UpgradeUIScript>().turretStats = this;
            upgradeUIObject.GetComponent<UpgradeUIScript>().player = interactingPlayer;
            
        interactingPlayer.GetComponentInChildren<RBController>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
        
        upgradeUIObject.GetComponent<UpgradeUIScript>().UpdateUi();
    }

    private void HandleLevelChanged(int oldValue, int newValue)
    {
        upgradeUIObject.GetComponent<UpgradeUIScript>().UpdateUi();
        Debug.Log($"Level changed from {turretLevels.levels[oldValue].level} to {turretLevels.levels[newValue].level}");
    }
}
