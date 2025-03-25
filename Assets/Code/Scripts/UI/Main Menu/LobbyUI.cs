using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_Text lobbyName;
    [SerializeField] private TMP_Text lobbySlots;
    [SerializeField] private TMP_Text lobbyCode;
    [SerializeField] private Toggle lobbyPrivate;
    [SerializeField] private RectTransform playerList;
    [SerializeField] private Button playerTemplate;
    [SerializeField] private LobbyController lobbyController;
    [SerializeField] private RelayController relayController;
    [SerializeField] private MainMenuCanvasController mainMenuCanvasController;
    [SerializeField] private Button startButton;

    [SerializeField] private Scene nextScene;
    
    private bool _isQuitting = false;
    
    public void Awake()
    {
        backButton.onClick.AddListener(OnBackButtonClicked);
        startButton.onClick.AddListener(OnStartButtonClicked);
    }

    public void OnEnable()
    {
        Application.wantsToQuit += OnWantsToQuit;
        lobbyController.OnLobbyInfoRefresh += RefreshLobbyInfo;
        lobbyController.OnLobbyInfoRefresh += CheckIfGameStarted;
        RefreshLobbyInfo();
    }

    public void OnDisable()
    {
        Application.wantsToQuit -= OnWantsToQuit;
        lobbyController.OnLobbyInfoRefresh -= RefreshLobbyInfo;
        lobbyController.OnLobbyInfoRefresh -= CheckIfGameStarted;
        ClearPlayerList();
        lobbyName.text = "";
        lobbySlots.text = "";
    }
    
    private bool OnWantsToQuit()
    {
        if (!_isQuitting)
        {
            _isQuitting = true;
            LeaveLobbyAndQuit();
            return false;
        }

        return true;
    }
    
    private async void LeaveLobbyAndQuit()
    {
        await LeaveLobby();
        Application.Quit();
    }

    private void ClearPlayerList()
    {
        foreach (Transform playerButton in playerList.transform)
        {
            Destroy(playerButton.gameObject);
        }
    }

    private async Task<bool> LeaveLobby()
    {
        try
        {
            await lobbyController.LeaveLobby();
            return true;
        }
        catch
        {
            mainMenuCanvasController.ShowMessage("There was a problem leaving the lobby. Please try again.");
        }
        return false;
    }

    private void RefreshLobbyInfo()
    {
        if (lobbyController.CurrentLobby == null) return;
        
        int usedSlots = lobbyController.CurrentLobby.MaxPlayers - lobbyController.CurrentLobby.AvailableSlots;
        Button playerButton;
        
        lobbyName.text = lobbyController.CurrentLobby.Name;
        lobbySlots.text = usedSlots + " / " + lobbyController.CurrentLobby.MaxPlayers;
        lobbyCode.text = lobbyController.CurrentLobby.LobbyCode;
        lobbyPrivate.isOn = lobbyController.CurrentLobby.IsPrivate;
        startButton.gameObject.SetActive(lobbyController.isHost);
        ClearPlayerList();
        foreach (Player player in lobbyController.CurrentLobby.Players)
        {
            playerButton = Instantiate(playerTemplate, playerList);
            playerButton.transform.GetChild(0).GetComponent<TMP_Text>().text = player.Id;
            playerButton.gameObject.SetActive(true);
        }
    }

    private async void CheckIfGameStarted()
    {
        if (lobbyController.CurrentLobby.Data.TryGetValue("relayCode", out DataObject relayCode))
        {
            if (relayCode.Value == null) return;

            if (!lobbyController.isHost)
            {
                JoinAllocation joinAllocation = await relayController.JoinAllocation(relayCode.Value);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                    joinAllocation.RelayServer.IpV4,
                    (ushort)joinAllocation.RelayServer.Port,
                    joinAllocation.AllocationIdBytes,
                    joinAllocation.Key,
                    joinAllocation.ConnectionData);
                SceneManager.LoadScene("Marcin K");
            }
        }
    }

    private async void OnStartButtonClicked()
    {
        try
        {
            await relayController.CreateAllocation(lobbyController.CurrentLobby.MaxPlayers);
            string code = await relayController.GetJoinCode();
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                relayController.CurrentAllocation.RelayServer.IpV4,
                (ushort)relayController.CurrentAllocation.RelayServer.Port,
                relayController.CurrentAllocation.AllocationIdBytes,
                relayController.CurrentAllocation.Key,
                relayController.CurrentAllocation.ConnectionData);
            SceneManager.LoadScene("Marcin K");
            await lobbyController.AddRelayCodeToLobby(code);
        }
        catch
        {
            mainMenuCanvasController.ShowMessage("There was a problem starting the game. Please try again.");
        }
    }
    
    private async void OnBackButtonClicked()
    {
        bool leftFromLobby = await LeaveLobby();
        if (leftFromLobby) gameObject.SetActive(false);
    }
}
