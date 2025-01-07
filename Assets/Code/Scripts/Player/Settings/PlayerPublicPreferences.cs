using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPreferences", menuName = "Custom Settings/Player Public Preferences")]
public class PlayerPublicPreferences : ScriptableObject
{
    public TurretManager.TurretType selectedTurret;
}
