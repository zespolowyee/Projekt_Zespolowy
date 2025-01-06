using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UpgradeTurret : NetworkBehaviour
{
    [SerializeField] private GameObject upgradeUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private TurretLevels turretLevels;
    private GameObject upgradeUIObject;
    private NetworkVariable<int> currentLevelIndex = new NetworkVariable<int>(0);
    private int maxLevelIndex;
    private TurretScript turretScript;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
            turretScript.SetTurretLevelServerRpc(GetCurrentLevel());

        currentLevelIndex.OnValueChanged += HandleLevelChanged;
    }

    private void Awake()
    {
        maxLevelIndex = turretLevels.levels.Count - 1;
        turretScript = gameObject.GetComponent<TurretScript>();
    }

    public Boolean CanUpgrade()
    {
        return currentLevelIndex.Value < maxLevelIndex;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpgradeServerRPC()
    {
        if (!CanUpgrade())
            return;

        currentLevelIndex.Value++;
        turretScript.SetTurretLevelServerRpc(GetCurrentLevel());
    }

    public TurretLevel GetCurrentLevel()
    {
        return turretLevels.levels[currentLevelIndex.Value];
    }

    public void DisplayUpgradeUI()
    {
        if(upgradeUIObject == null)
            upgradeUIObject = Instantiate(upgradeUI);
        upgradeUIObject.GetComponent<UpgradeUIScript>().DisplayNewLevel(turretLevels.levels[currentLevelIndex.Value]);
        UpgradeServerRPC();
        //UpgradeServerRPC();
        //Debug.Log(GetCurrentLevel().level);
    }

    private void HandleLevelChanged(int oldValue, int newValue)
    {
        upgradeUIObject.GetComponent<UpgradeUIScript>().DisplayNewLevel(turretLevels.levels[newValue]);
        Debug.Log($"Level changed from {turretLevels.levels[oldValue].level} to {turretLevels.levels[newValue].level}");
    }
}
