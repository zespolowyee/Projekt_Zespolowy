using Unity.Netcode;
using UnityEngine;

public class PlayerClass : NetworkBehaviour
{
    public string className;
    public float attackRange;

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