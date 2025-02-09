using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject knightPrefab;
    [SerializeField] private GameObject paladinPrefab;

    private void Start()
    {
        // Subscribe to class selection event
        SelectClassUI.OnClassSelected += OnClassSelected;
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        SelectClassUI.OnClassSelected -= OnClassSelected;
    }

    private void OnClassSelected(string className)
    {
        if (NetworkManager.Singleton.IsClient && NetworkManager.Singleton.IsHost)
        {
            SpawnPlayer(className, NetworkManager.Singleton.LocalClientId);
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            // Tell the server to spawn the selected class
            SpawnPlayerServerRpc(className);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(string className, ServerRpcParams rpcParams = default)
    {
        SpawnPlayer(className, rpcParams.Receive.SenderClientId);
    }

    private void SpawnPlayer(string className, ulong clientId)
    {
        GameObject playerPrefab = null;
        Debug.Log("Spawning: " + className);

        switch (className)
        {
            case "Knight":
                playerPrefab = knightPrefab;
                break;
            case "Paladin":
                playerPrefab = paladinPrefab;
                break;
        }

        if (playerPrefab != null)
        {
            GameObject playerInstance = Instantiate(playerPrefab);
            playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        }
    }
}
