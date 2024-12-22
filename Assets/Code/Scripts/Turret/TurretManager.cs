// class that holds enums for the different types of turrets and returns the correct turret prefab based on the enum

using UnityEngine;

public static class TurretManager
{
    public enum TurretType
    {
        TurretCannon
    }

    public static GameObject GetTurretPrefab(TurretType turretType)
    {
        switch (turretType)
        {
            case TurretType.TurretCannon:
                Debug.Log("Turret_Cannon");
                return Resources.Load<GameObject>("Turrets/Turret_Cannon");
            default:
                return null;
        }
    }
}