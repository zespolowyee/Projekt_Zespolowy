using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class LobbyController : MonoBehaviour
{
    public Lobby CurrentLobby;
    public bool isHost = false;
    public delegate void LobbyInfoRefresh();
    public event LobbyInfoRefresh OnLobbyInfoRefresh;
    
    private ILobbyEvents _lobbyEvents;
    private LobbyEventCallbacks _lobbyEventCallbacks;
    
    public async void Start()
    {
        DontDestroyOnLoad(this);
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        _lobbyEventCallbacks = new LobbyEventCallbacks();
        _lobbyEventCallbacks.LobbyChanged += OnLobbyChanged;
    }

    public string GetMyPlayerId()
    {
        return AuthenticationService.Instance.PlayerId;
    }

    public async Task CreateLobby(string lobbyName, int maxPlayers, bool isPrivate)
    {
        CreateLobbyOptions options = new CreateLobbyOptions();
        options.IsPrivate = isPrivate;
        options.Data = new Dictionary<string, DataObject>
        {
            {"relayCode", new DataObject(DataObject.VisibilityOptions.Member, null, DataObject.IndexOptions.S1)}
        };
        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
        CurrentLobby = lobby;
        isHost = true;
        await RegisterCallbacks(lobby.Id);
        StartCoroutine(Heartbeat());
    }

    public async Task AddRelayCodeToLobby(string relayCode)
    {
        UpdateLobbyOptions options = new UpdateLobbyOptions();
        options.Data = new Dictionary<string, DataObject>
        {
            {"relayCode", new DataObject(DataObject.VisibilityOptions.Member, relayCode, DataObject.IndexOptions.S1)}
        };
        
        await LobbyService.Instance.UpdateLobbyAsync(CurrentLobby.Id, options);
    }

    public async Task JoinLobby(string lobbyId)
    {
        Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
        CurrentLobby = lobby;
        isHost = lobby.HostId == GetMyPlayerId();
        await RegisterCallbacks(lobby.Id);
    }
    
    public async Task JoinLobbyWithCode(string lobbyCode)
    {
        Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
        CurrentLobby = lobby;
        isHost = lobby.HostId == GetMyPlayerId();
        await RegisterCallbacks(lobby.Id);
    }
    
    public async Task LeaveLobby()
    {
        if (CurrentLobby == null)
            return;
        
        await UnregisterCallbacks();
        await LobbyService.Instance.RemovePlayerAsync(CurrentLobby.Id, GetMyPlayerId());
        CurrentLobby = null;
        isHost = false;
    }

    public async Task<QueryResponse> ListLobbies()
    {
        QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
        return queryResponse;
    }

    public async Task RegisterCallbacks(string lobbyId)
    {
        _lobbyEvents = await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobbyId, _lobbyEventCallbacks);
    }
    
    public async Task UnregisterCallbacks()
    {
        if (_lobbyEvents != null)
        {
            await _lobbyEvents.UnsubscribeAsync();
            _lobbyEvents = null;
        }
    }

    public void OnLobbyChanged(ILobbyChanges lobbyChanges)
    {
        if (CurrentLobby == null) return;
        lobbyChanges.ApplyToLobby(CurrentLobby);
        isHost = CurrentLobby.HostId == GetMyPlayerId();
        OnLobbyInfoRefresh?.Invoke();
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
}
