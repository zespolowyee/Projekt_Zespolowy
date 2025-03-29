using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawn : MonoBehaviour
{
    void Start()
    {
        LobbyController lobbyController = FindFirstObjectByType<LobbyController>();
        if (lobbyController.IsHost)
        {
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            NetworkManager.Singleton.StartClient();
        }
    }
}
