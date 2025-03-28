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
    
    public void Start()
    {
        DontDestroyOnLoad(this);
        _lobbyEventCallbacks = new LobbyEventCallbacks();
        _lobbyEventCallbacks.LobbyChanged += OnLobbyChanged;
    }

    public string GetMyPlayerId()
    {
        return AuthenticationService.Instance.PlayerId;
    }

    public async Task CreateLobby(string lobbyName, int maxPlayers, bool isPrivate)
    {
        CreateLobbyOptions options = new CreateLobbyOptions
        {
            IsPrivate = isPrivate,
            Player = new Player
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {"playerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, AuthenticationService.Instance.PlayerName)}
                }
            },
            Data = new Dictionary<string, DataObject>
            {
                {"relayCode", new DataObject(DataObject.VisibilityOptions.Member, null, DataObject.IndexOptions.S1)}
            }
        };
        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
        CurrentLobby = lobby;
        isHost = true;
        await RegisterCallbacks(lobby.Id);
        StartCoroutine(Heartbeat());
    }

    public async Task AddRelayCodeToLobby(string relayCode)
    {
        UpdateLobbyOptions options = new UpdateLobbyOptions
        {
            Data = new Dictionary<string, DataObject>
            {
                {"relayCode", new DataObject(DataObject.VisibilityOptions.Member, relayCode, DataObject.IndexOptions.S1)}
            }
        };

        await LobbyService.Instance.UpdateLobbyAsync(CurrentLobby.Id, options);
    }

    public async Task JoinLobby(string lobbyId)
    {
        JoinLobbyByIdOptions options = new JoinLobbyByIdOptions
        {
            Player = new Player
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {"playerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, AuthenticationService.Instance.PlayerName)}
                }
            }
        };
        Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, options);
        CurrentLobby = lobby;
        isHost = lobby.HostId == GetMyPlayerId();
        await RegisterCallbacks(lobby.Id);
    }
    
    public async Task JoinLobbyWithCode(string lobbyCode)
    {
        JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions
        {
            Player = new Player
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {"playerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, AuthenticationService.Instance.PlayerName)}
                }
            }
        };
        Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, options);
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
        QueryLobbiesOptions options = new QueryLobbiesOptions
        {
            Filters = new List<QueryFilter>
            {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
            }
        };
        QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(options);
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
