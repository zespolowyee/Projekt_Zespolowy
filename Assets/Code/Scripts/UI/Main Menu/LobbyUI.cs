using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_Text lobbyName;
    [SerializeField] private TMP_Text lobbySlots;
    [SerializeField] private TMP_Text lobbyCode;
    [SerializeField] private TMP_Text lobbyMap;
    [SerializeField] private TMP_Dropdown lobbyMapHost;
    [SerializeField] private Toggle lobbyPrivate;
    [SerializeField] private RectTransform playerList;
    [SerializeField] private Button playerTemplate;
    [SerializeField] private RectTransform selectClassContainer;
    [SerializeField] private RectTransform classList;
    [SerializeField] private Button classTemplate;
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
        lobbyMapHost.onValueChanged.AddListener(OnMapChange);
        FillMapDropdown();
        FillClassList();
    }

    public void OnEnable()
    {
        Application.wantsToQuit += OnWantsToQuit;
        lobbyController.OnLobbyInfoRefresh += RefreshLobbyInfo;
        lobbyController.OnLobbyInfoRefresh += CheckIfGameStarted;
        lobbyController.OnConnectionLost += OnConnectionLost;
        lobbyController.OnConnectionRestored += OnConnectionRestored;
        RefreshLobbyInfo();
        CheckIfGameStarted();
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

    private void FillMapDropdown()
    {
        Array values = Enum.GetValues(typeof(Map));
        lobbyMapHost.options = new List<TMP_Dropdown.OptionData>();
        string map;
        for (int i = 0; i < values.Length; i++)
        {
            map = MapExtensions.GetName((Map)values.GetValue(i));
            lobbyMapHost.options.Add(new TMP_Dropdown.OptionData(map));
        }
    }
    
    private void FillClassList()
    {
        Button classButton;
        Array values = Enum.GetValues(typeof(PlayerClassType));
        foreach (PlayerClassType playerClass in values)
        {
            classButton = Instantiate(classTemplate, classList);
            classButton.transform.GetChild(0).GetComponent<TMP_Text>().text = playerClass.ToString();
            classButton.onClick.AddListener(() => ChangeMyPlayerClass(playerClass));
            classButton.gameObject.SetActive(true);
        }
    }

    private async void ChangeMyPlayerClass(PlayerClassType playerClass)
    {
        if (lobbyController.CurrentLobby == null)
        {
            selectClassContainer.gameObject.SetActive(false);
            return;
        }

        var myPlayer = lobbyController.GetMyPlayer();
        if (myPlayer.Data.TryGetValue(lobbyController.playerClassVariableName, out PlayerDataObject currentClass) &&
            currentClass.Value == playerClass.ToString())
        {
            selectClassContainer.gameObject.SetActive(false);
            return;
        }

        try
        {
            await lobbyController.ChangeMyPlayerClass(playerClass);
        }
        catch(LobbyServiceException ex)
        {
            switch (ex.Reason)
            {
                case LobbyExceptionReason.NetworkError:
                    mainMenuCanvasController.ShowMessage("Check your internet connection.");
                    break;
                default:
                    mainMenuCanvasController.ShowMessage("There was an unknown problem while changing the class. Please try again.");
                    break;
            }
            Debug.LogException(ex);
        }
        
        selectClassContainer.gameObject.SetActive(false);
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
            mainMenuCanvasController.ShowMessage("There was an unknown problem while leaving the lobby. Please try again.");
            Debug.LogException(ex);
        }
        return false;
    }

    private void RefreshLobbyDetails()
    {
        int usedSlots = lobbyController.CurrentLobby.MaxPlayers - lobbyController.CurrentLobby.AvailableSlots;
        
        lobbyName.text = lobbyController.CurrentLobby.Name;
        lobbySlots.text = usedSlots + " / " + lobbyController.CurrentLobby.MaxPlayers;
        lobbyCode.text = lobbyController.CurrentLobby.LobbyCode;
        lobbyPrivate.isOn = lobbyController.CurrentLobby.IsPrivate;
        startButton.gameObject.SetActive(lobbyController.IsHost);
    }
    
    private void RefreshMapInfo()
    {
        lobbyController.CurrentLobby.Data.TryGetValue(lobbyController.mapVariableName, out DataObject dataObject);
        string currentMap = dataObject.Value;
        if (lobbyController.IsHost)
        {
            lobbyMap.gameObject.SetActive(false);
            lobbyMapHost.gameObject.SetActive(true);
            Array values = Enum.GetValues(typeof(Map));
            string map;
            for (int i = 0; i < values.Length; i++)
            {
                map = MapExtensions.GetName((Map)values.GetValue(i));
                if (map == currentMap)
                {
                    lobbyMapHost.value = i;
                }
            }
        }
        else
        {
            lobbyMap.gameObject.SetActive(true);
            lobbyMapHost.gameObject.SetActive(false);
            lobbyMap.text = currentMap;
        }
    }
    
    private void RefreshPlayerList()
    {
        Button playerButton;
        String playerName;
        String playerClass;
        
        ClearPlayerList();
        
        foreach (Player player in lobbyController.CurrentLobby.Players)
        {
            playerName = player.Data.TryGetValue(lobbyController.playerNameVariableName, out PlayerDataObject playerNameDataObject)
                ? playerNameDataObject.Value
                : "Error";
            
            playerClass = player.Data.TryGetValue(lobbyController.playerClassVariableName, out PlayerDataObject playerClassDataObject)
                ? playerClassDataObject.Value
                : "Error";
            
            playerButton = Instantiate(playerTemplate, playerList);
            if(player.Id == lobbyController.GetMyPlayerId())
            {
                playerButton.onClick.AddListener(() =>
                {
                    var currentState = selectClassContainer.gameObject.activeSelf;
                    selectClassContainer.gameObject.SetActive(!currentState);
                });
            }
            playerButton.transform.GetChild(0).GetComponent<TMP_Text>().text = playerName;
            playerButton.transform.GetChild(1).GetComponent<TMP_Text>().text = playerClass;
            playerButton.gameObject.SetActive(true);
        }
    }

    private void RefreshLobbyInfo()
    {
        if (lobbyController.CurrentLobby == null) return;
        
        RefreshLobbyDetails();
        RefreshMapInfo();
        RefreshPlayerList();
    }
    
    private async void OnMapChange(int mapIdx)
    {
        if (!lobbyController.IsHost)
            return;
        
        Array values = Enum.GetValues(typeof(Map));
        Map selectedMap = (Map)values.GetValue(mapIdx);
        
        await lobbyController.ChangeMap(selectedMap);
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
        if (lobbyController.CurrentLobby == null) return;
        
        if (lobbyController.CurrentLobby.Data.TryGetValue(lobbyController.relayCodeVariableName, out DataObject relayCode))
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

                    lobbyController.CurrentLobby.Data.TryGetValue(lobbyController.mapVariableName, out DataObject dataObject);
                    
                    SceneManager.LoadScene(dataObject.Value);
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
            
            lobbyController.CurrentLobby.Data.TryGetValue(lobbyController.mapVariableName, out DataObject dataObject);
                    
            SceneManager.LoadScene(dataObject.Value);
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
