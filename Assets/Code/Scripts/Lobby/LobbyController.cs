using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyController : MonoBehaviour
{
    [SerializeField] private MainMenuCanvasController mainMenuCanvasController; 
    public async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async Task CreateLobby(string lobbyName, int maxPlayers)
    {
        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);
        Debug.Log("Created Lobby! " + lobby.Name + " " + lobby.MaxPlayers);
    }

    public async Task<QueryResponse> ListLobbies()
    {
        QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
        return queryResponse;
    }
}
