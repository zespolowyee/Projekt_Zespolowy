using System;
using System.Collections;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEditor.MemoryProfiler;
using UnityEngine;

// Ten skrypt spawnuje gracza w zależności od jego wyboru klasy.
// W przypadku dodania nowej klasy, uwzględnij ją w metodzie SpawnPlayer.
public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private PlayerClassPrefabMapping playerClassPrefabMapping;

    private void Start()
    {
        // Subscribe to class selection event
        SelectClassUI.OnClassSelected += OnClassSelected;
        
        //If the game was stared from a lobby. Try to get class from lobby controller.
        LobbyController lobbyController = FindFirstObjectByType<LobbyController>();
        if (lobbyController != null)
        {
            var myPlayer = lobbyController.GetMyPlayer();
            if (myPlayer.Data.TryGetValue(lobbyController.playerClassVariableName, out PlayerDataObject currentClassObject))
            {
                PlayerClassType currentClass = Enum.Parse<PlayerClassType>(currentClassObject.Value);
                NetworkManager.Singleton.OnConnectionEvent += (manager, eventData) =>
                {
                    if (eventData.EventType != ConnectionEvent.ClientConnected)
                        return;

                    if (eventData.ClientId == NetworkManager.Singleton.LocalClientId)
                    {
                        OnClassSelected(currentClass);   
                    }
                };
                
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
    }

    public override void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        SelectClassUI.OnClassSelected -= OnClassSelected;
        base.OnDestroy();
    }

    private void OnClassSelected(PlayerClassType playerClass)
    {
        if (NetworkManager.Singleton.IsClient && NetworkManager.Singleton.IsHost)
        {
            SpawnPlayer(playerClass, NetworkManager.Singleton.LocalClientId);
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            // Tell the server to spawn the selected class
            SpawnPlayerServerRpc(playerClass);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(PlayerClassType playerClass, ServerRpcParams rpcParams = default)
    {
        SpawnPlayer(playerClass, rpcParams.Receive.SenderClientId);
    }


    private void SpawnPlayer(PlayerClassType playerClass, ulong clientId)
    {
        GameObject playerPrefab = null;
        Debug.Log("Spawning: " + playerClass.ToString());

        playerPrefab = playerClassPrefabMapping.GetPrefab(playerClass);

        if (playerPrefab != null)
        {
            GameObject playerInstance = Instantiate(playerPrefab);
            playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        }
    }
}
