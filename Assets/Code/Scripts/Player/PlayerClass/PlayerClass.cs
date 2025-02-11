using Unity.Netcode;
using UnityEngine;

// Klasa generyczna ze statystykami postaci gracza.
// Dziedzicz po tej klasie, aby dodać nową klasę postaci.
public class PlayerClass : NetworkBehaviour
{
    public string className;
    public float attackRange;
    public int damage;

    [ServerRpc]
    public void SetClassNameServerRpc(string newClassName)
    {
        if (IsServer)
        {
            className = newClassName;
            // Notify all clients about the new className
            UpdateClassNameClientRpc(className);
        }
    }

    [ClientRpc]
    private void UpdateClassNameClientRpc(string updatedClassName)
    {
        className = updatedClassName;
        Debug.Log($"Class name updated to: {className}");
    }
}