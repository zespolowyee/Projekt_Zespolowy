using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_Text lobbyName;
    [SerializeField] private TMP_Text lobbySlots;
    [SerializeField] private RectTransform playerList;
    [SerializeField] private Button playerTemplate;
    [SerializeField] private LobbyController lobbyController;
    [SerializeField] private MainMenuCanvasController mainMenuCanvasController;
    [SerializeField] private Button startButton;
    
    private bool isQuitting = false;
    
    public void Awake()
    {
        backButton.onClick.AddListener(OnBackButtonClicked);
        startButton.onClick.AddListener(OnStartButtonClicked);
        Application.wantsToQuit += OnWantsToQuit;
    }
    
    public void OnEnable()
    {
        lobbyController.OnLobbyInfoRefresh += RefreshLobbyInfo;
        RefreshLobbyInfo();
    }

    public void OnDisable()
    {
        lobbyController.OnLobbyInfoRefresh -= RefreshLobbyInfo;
        ClearPlayerList();
        lobbyName.text = "";
        lobbySlots.text = "";
    }
    
    private bool OnWantsToQuit()
    {
        if (!isQuitting)
        {
            isQuitting = true;
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

    private async Task LeaveLobby()
    {
        try
        {
            await lobbyController.LeaveLobby();
        }
        catch
        {
            mainMenuCanvasController.ShowMessage("There was a problem leaving the lobby. Please try again.");
        }
    }

    private void RefreshLobbyInfo()
    {
        int usedSlots = lobbyController.CurrentLobby.MaxPlayers - lobbyController.CurrentLobby.AvailableSlots;
        Button playerButton;
        
        lobbyName.text = lobbyController.CurrentLobby.Name;
        lobbySlots.text = usedSlots + " / " + lobbyController.CurrentLobby.MaxPlayers;
        startButton.gameObject.SetActive(lobbyController.IsHost);
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
        await LeaveLobby();
        gameObject.SetActive(false);
    }
}
