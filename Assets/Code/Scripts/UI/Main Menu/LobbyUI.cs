using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
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
    [SerializeField] private MainMenuCanvasController mainMenuCanvasController;
    [SerializeField] private Button startButton;

    [SerializeField] private Scene nextScene;
    private Coroutine timeout;
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
        lobbyController.OnConnectionLost += OnConnectionLost;
        lobbyController.OnConnectionRestored += OnConnectionRestored;
        RefreshLobbyInfo();
    }

    public void OnDisable()
    {
        Application.wantsToQuit -= OnWantsToQuit;
        lobbyController.OnLobbyInfoRefresh -= RefreshLobbyInfo;
        lobbyController.OnLobbyInfoRefresh -= CheckIfGameStarted;
        lobbyController.OnConnectionLost -= OnConnectionLost;
        lobbyController.OnConnectionRestored -= OnConnectionRestored;
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
        catch (Exception ex)
        {
            mainMenuCanvasController.ShowMessage("There was a problem leaving the lobby. Please try again.");
            Debug.LogException(ex);
        }
        return false;
    }

    private void RefreshLobbyInfo()
    {
        if (lobbyController.CurrentLobby == null) return;
        
        int usedSlots = lobbyController.CurrentLobby.MaxPlayers - lobbyController.CurrentLobby.AvailableSlots;
        Button playerButton;
        String playerName;
        
        lobbyName.text = lobbyController.CurrentLobby.Name;
        lobbySlots.text = usedSlots + " / " + lobbyController.CurrentLobby.MaxPlayers;
        lobbyCode.text = lobbyController.CurrentLobby.LobbyCode;
        lobbyPrivate.isOn = lobbyController.CurrentLobby.IsPrivate;
        startButton.gameObject.SetActive(lobbyController.IsHost);
        ClearPlayerList();
        foreach (Player player in lobbyController.CurrentLobby.Players)
        {
            playerName = player.Data.TryGetValue("playerName", out PlayerDataObject playerDataObject)
                ? playerDataObject.Value
                : "Nameless player";
            playerButton = Instantiate(playerTemplate, playerList);
            playerButton.transform.GetChild(0).GetComponent<TMP_Text>().text = playerName;
            playerButton.gameObject.SetActive(true);
        }
    }
    
    private void OnConnectionLost()
    {
        startButton.interactable = false;
        timeout = StartCoroutine(Timeout(30));
    }
    
    private void OnConnectionRestored()
    {
        Debug.Log("OnConnectionRestored");
        startButton.interactable = true;
        StopCoroutine(timeout);
        mainMenuCanvasController.ShowMessage("Connection restored");
        RefreshLobbyInfo();
    }

    private async void CheckIfGameStarted()
    {
        if (lobbyController.CurrentLobby.Data.TryGetValue("relayCode", out DataObject relayCode))
        {
            if (relayCode.Value == null) return;

            if (!lobbyController.IsHost)
            {
                try
                {
                    JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayCode.Value);
                    NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                        joinAllocation.RelayServer.IpV4,
                        (ushort)joinAllocation.RelayServer.Port,
                        joinAllocation.AllocationIdBytes,
                        joinAllocation.Key,
                        joinAllocation.ConnectionData,
                        joinAllocation.HostConnectionData);
                    SceneManager.LoadScene("Marcin K");
                }
                catch (RelayServiceException ex)
                {
                    switch (ex.Reason)
                    {
                        case RelayExceptionReason.NetworkError:
                            mainMenuCanvasController.ShowMessage("Check your internet connection.\nWill try to join in 2 seconds.");
                            break;
                        default:
                            mainMenuCanvasController.ShowMessage("There was an unknown problem while joining the game.\nWill try to join in 2 seconds.");
                            break;
                    }
                    
                    StartCoroutine(TryToJoinAgain());
                    Debug.LogException(ex);
                }
            }
        }
    }

    private async void OnStartButtonClicked()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(lobbyController.CurrentLobby.MaxPlayers);
            string code = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData);
            SceneManager.LoadScene("Marcin K");
            await lobbyController.AddRelayCodeToLobby(code);
        }
        catch (RelayServiceException ex)
        {
            switch (ex.Reason)
            {
                case RelayExceptionReason.NetworkError:
                    mainMenuCanvasController.ShowMessage("Check your internet connection.");
                    break;
                default:
                    mainMenuCanvasController.ShowMessage("There was an unknown problem while starting the game. Please try again.");
                    break;
            }
            Debug.LogException(ex);
        }
    }
    
    private async void OnBackButtonClicked()
    {
        bool leftFromLobby = await LeaveLobby();
        if (leftFromLobby) gameObject.SetActive(false);
    }

    private IEnumerator Timeout(int seconds)
    {
        var delay = new WaitForSecondsRealtime(1);
        int counter = seconds;
        
        while (counter > 0)
        {
            mainMenuCanvasController.ShowMessage($"Connection lost\nTime left before disconnecting: {counter}s");
            yield return delay;
            counter--;
        }
        mainMenuCanvasController.ShowMessage($"Disconnected from the lobby");
        OnBackButtonClicked();
    }

    private IEnumerator TryToJoinAgain()
    {
        yield return new WaitForSeconds(2);
        CheckIfGameStarted();
    }
}
