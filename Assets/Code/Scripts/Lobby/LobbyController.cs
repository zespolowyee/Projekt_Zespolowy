using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Multiplayer;
using Unity.VisualScripting;
using UnityEngine;

public class LobbyController : MonoBehaviour
{
    //Public atributes
    public Lobby CurrentLobby;
    public bool IsHost{
        get => _isHost;
        internal set
        {
            if (_isHost == false && value == true)
                _heartbeat = StartCoroutine(Heartbeat());
            
            if (_isHost == true && value == false)
                if(_heartbeat != null)
                    StopCoroutine(_heartbeat);
            
            _isHost = value;
        }
    }
    
    
    //Events
    public delegate void LobbyInfoRefresh();
    public event LobbyInfoRefresh OnLobbyInfoRefresh;
    
    public delegate void ConnectionLost();
    public event ConnectionLost OnConnectionLost;
    
    public delegate void ConnectionRestored();
    public event ConnectionRestored OnConnectionRestored;
    
    //Private atributes
    private ILobbyEvents _lobbyEvents;
    private LobbyEventCallbacks _lobbyEventCallbacks;
    private LobbyEventConnectionState? _lastConnectionState;
    private bool _isConnected = false;
    private Coroutine _heartbeat;
    private bool _isHost = false;
    
    public void Start()
    {
        DontDestroyOnLoad(this);
        _lobbyEventCallbacks = new LobbyEventCallbacks();
        _lobbyEventCallbacks.LobbyChanged += OnLobbyChanged;
        _lobbyEventCallbacks.LobbyEventConnectionStateChanged += OnConnectionStateChanged;
    }

    private string GetMyPlayerId()
    {
        return AuthenticationService.Instance.PlayerId;
    }
    
    private string GetMyPlayerName()
    {
        return AuthenticationService.Instance.PlayerName;
    }

    private Player GetDefaultPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                {"playerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, GetMyPlayerName())}
            }
        };
    }
    
    private Dictionary<string, DataObject> GetDefaultLobbyData()
    {
        return new Dictionary<string, DataObject>
        {
            { "relayCode", new DataObject(DataObject.VisibilityOptions.Member, null, DataObject.IndexOptions.S1)},
            { "map", new DataObject(DataObject.VisibilityOptions.Member, MapExtensions.GetName(Map.MarcinK), DataObject.IndexOptions.S2)},
        };
    }

    public async Task CreateLobby(string lobbyName, int maxPlayers, bool isPrivate)
    {
        CreateLobbyOptions options = new CreateLobbyOptions
        {
            IsPrivate = isPrivate,
            Player = GetDefaultPlayer(),
            Data = GetDefaultLobbyData()
        };
        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
        CurrentLobby = lobby;
        IsHost = true;
        await RegisterCallbacks(lobby.Id);
    }

    public async Task AddRelayCodeToLobby(string relayCode)
    {
        if (CurrentLobby == null)
            return;
        
        UpdateLobbyOptions options = new UpdateLobbyOptions
        {
            Data = new Dictionary<string, DataObject>
            {
                {"relayCode", new DataObject(DataObject.VisibilityOptions.Member, relayCode, DataObject.IndexOptions.S1)}
            }
        };

        await LobbyService.Instance.UpdateLobbyAsync(CurrentLobby.Id, options);
    }
    
    public async Task ChangeMap(Map map)
    {
        if (CurrentLobby == null)
            return;
        
        UpdateLobbyOptions options = new UpdateLobbyOptions
        {
            Data = new Dictionary<string, DataObject>
            {
                {"map", new DataObject(DataObject.VisibilityOptions.Member, MapExtensions.GetName(map), DataObject.IndexOptions.S2)}
            }
        };

        await LobbyService.Instance.UpdateLobbyAsync(CurrentLobby.Id, options);
    }

    public async Task JoinLobby(string lobbyId)
    {
        JoinLobbyByIdOptions options = new JoinLobbyByIdOptions
        {
            Player = GetDefaultPlayer()
        };
        Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, options);
        CurrentLobby = lobby;
        IsHost = lobby.HostId == GetMyPlayerId();
        await RegisterCallbacks(lobby.Id);
    }
    
    public async Task JoinLobbyWithCode(string lobbyCode)
    {
        JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions
        {
            Player = GetDefaultPlayer()
        };
        Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, options);
        CurrentLobby = lobby;
        IsHost = lobby.HostId == GetMyPlayerId();
        await RegisterCallbacks(lobby.Id);
    }
    
    public async Task LeaveLobby()
    {
        if (CurrentLobby == null)
            return;
        
        await UnregisterCallbacks();
        if(_isConnected)
            await LobbyService.Instance.RemovePlayerAsync(CurrentLobby.Id, GetMyPlayerId());
        CurrentLobby = null;
        IsHost = false;
        _isConnected = false;
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

    private async Task RegisterCallbacks(string lobbyId)
    {
        _lobbyEvents = await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobbyId, _lobbyEventCallbacks);
    }
    
    private async Task UnregisterCallbacks()
    {
        if (_lobbyEvents != null)
        {
            await _lobbyEvents.UnsubscribeAsync();
            _lobbyEvents = null;
        }
    }
    
    private async void OnConnectionStateChanged(LobbyEventConnectionState state)
    {
        Debug.Log("OnConnectionStateChanged " + state);
        
        if (state == LobbyEventConnectionState.Unsynced){
            OnConnectionLost?.Invoke();
            _isConnected = false;
        }
        
        if (state == LobbyEventConnectionState.Subscribed)
            _isConnected = true;

        if (state == LobbyEventConnectionState.Subscribed && _lastConnectionState is LobbyEventConnectionState.Unsynced){
            CurrentLobby = await LobbyService.Instance.GetLobbyAsync(CurrentLobby.Id);
            IsHost = CurrentLobby.HostId == GetMyPlayerId();
            foreach (Player player in CurrentLobby.Players)
            {
                Debug.Log("Player status: " + player.AllocationId);
            }
            Debug.Log(IsHost);
            OnConnectionRestored?.Invoke();
        }
        
        _lastConnectionState = state;
    }

    private void OnLobbyChanged(ILobbyChanges lobbyChanges)
    {
        Debug.Log("updated lobby changes");
        if (CurrentLobby == null)
            return;
        lobbyChanges.ApplyToLobby(CurrentLobby);
        IsHost = CurrentLobby.HostId == GetMyPlayerId();
        OnLobbyInfoRefresh?.Invoke();
    }

    private IEnumerator Heartbeat()
    {
        var delay = new WaitForSecondsRealtime(15);
        
        while (CurrentLobby != null)
        {
            if (_isConnected)
            {
                Debug.Log("heartbeat");
                LobbyService.Instance.SendHeartbeatPingAsync(CurrentLobby.Id);
            }
            yield return delay;
        }
    }
}
