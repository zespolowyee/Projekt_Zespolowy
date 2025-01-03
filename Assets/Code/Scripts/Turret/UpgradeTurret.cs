using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UpgradeTurret : MonoBehaviour
{
    [SerializeField] private GameObject upgradeUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private TurretLevels turretLevels;
    private GameObject upgradeUIObject;
    private int currentLevelIndex;
    private int maxLevelIndex;
    private ITurretScript turretScript;
    


    private void Awake()
    {
        currentLevelIndex = 0;
        maxLevelIndex = turretLevels.levels.Count - 1;
        turretScript = gameObject.GetComponent<ITurretScript>();
    }

    public void DisplayUpgradeUI()
    {
        if (currentLevelIndex <= maxLevelIndex)
        {
            turretScript.SetDamage(turretLevels.levels[currentLevelIndex].damage)
                .SetDetectionRange(turretLevels.levels[currentLevelIndex].range)
                .SetShootingInterval(turretLevels.levels[currentLevelIndex].shootingInterval);
            currentLevelIndex++;
            //upgradeUIObject = Instantiate(upgradeUI);
        }
    }
}
