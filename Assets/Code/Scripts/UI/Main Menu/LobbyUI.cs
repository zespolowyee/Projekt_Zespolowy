using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
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
        RefreshLobbyInfo();
    }

    public void OnDisable()
    {
        Application.wantsToQuit -= OnWantsToQuit;
        lobbyController.OnLobbyInfoRefresh -= RefreshLobbyInfo;
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

    private void OnStartButtonClicked()
    {
        //RefreshLobbyList();
    }
    
    private async void OnBackButtonClicked()
    {
        bool leftFromLobby = await LeaveLobby();
        if (leftFromLobby) gameObject.SetActive(false);
    }
}
