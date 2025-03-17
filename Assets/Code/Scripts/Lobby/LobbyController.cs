using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyController : MonoBehaviour
{
    [SerializeField] private MainMenuCanvasController mainMenuCanvasController;
    public Lobby CurrentLobby;
    public bool IsHost = false;
    public delegate void LobbyInfoRefresh();
    public event LobbyInfoRefresh OnLobbyInfoRefresh;
    
    public async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public string GetMyPlayerId()
    {
        return AuthenticationService.Instance.PlayerId;
    }

    public async Task CreateLobby(string lobbyName, int maxPlayers)
    {
        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);
        CurrentLobby = lobby;
        IsHost = true;
        StartCoroutine(Heartbeat());
        StartCoroutine(LobbyRefresh());
    }

    public async Task JoinLobby(string lobbyId)
    {
        Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
        CurrentLobby = lobby;
        IsHost = lobby.HostId == GetMyPlayerId();
        StartCoroutine(LobbyRefresh());
    }
    
    public async Task LeaveLobby()
    {
        if (CurrentLobby == null)
            return;
        
        await LobbyService.Instance.RemovePlayerAsync(CurrentLobby.Id, GetMyPlayerId());
        CurrentLobby = null;
        IsHost = false;
    }

    public async Task<QueryResponse> ListLobbies()
    {
        QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
        return queryResponse;
    }
    
    private IEnumerator Heartbeat()
    {
        var delay = new WaitForSecondsRealtime(15);
        
        while (CurrentLobby != null)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(CurrentLobby.Id);
            yield return delay;
        }
    }
    
    private IEnumerator LobbyRefresh()
    {
        var delay = new WaitForSecondsRealtime(1.1f);

        while (CurrentLobby != null)
        {
            RefreshLobbyAsync();
            yield return delay;
        }
    }
    
    private async void RefreshLobbyAsync()
    {
        try
        {
            CurrentLobby = await LobbyService.Instance.GetLobbyAsync(CurrentLobby.Id);
            IsHost = CurrentLobby.HostId == GetMyPlayerId();
            OnLobbyInfoRefresh();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error refreshing lobby: {e.Message}");
        }
    }
}
