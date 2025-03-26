using System;
using System.Collections;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class FindLobbyUI : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private RectTransform lobbyList;
    [SerializeField] private Button lobbyTemplate;
    [SerializeField] private LobbyController lobbyController;
    [SerializeField] private MainMenuCanvasController mainMenuCanvasController;
    [SerializeField] private Button refreshButton;
    
    public void Awake()
    {
        backButton.onClick.AddListener(OnBackButtonClicked);
        refreshButton.onClick.AddListener(OnRefreshButtonClicked);
    }

    public void OnEnable()
    {
        RefreshLobbyList();
    }

    public void OnDisable()
    {
        ClearLobbyList();
    }

    private void OnBackButtonClicked()
    {
        gameObject.SetActive(false);
    }
    
    private void OnRefreshButtonClicked()
    {
        RefreshLobbyList();
        StartCoroutine(ButtonCooldown(refreshButton, 1));
    }

    private void ClearLobbyList()
    {
        foreach (Transform lobbyButton in lobbyList.transform)
        {
            Destroy(lobbyButton.gameObject);
        }
    }

    private async void JoinLobby(string lobbyId)
    {
        try
        {
            await lobbyController.JoinLobby(lobbyId);
            mainMenuCanvasController.ShowLobby();
        }
        catch (LobbyServiceException ex)
        {
            switch (ex.Reason)
            {
                case LobbyExceptionReason.NetworkError:
                    mainMenuCanvasController.ShowMessage("Check your internet connection.");
                    break;
                case LobbyExceptionReason.LobbyFull:
                    mainMenuCanvasController.ShowMessage("This lobby is full.");
                    break;
                case LobbyExceptionReason.LobbyNotFound:
                    mainMenuCanvasController.ShowMessage("This lobby cannot be found.");
                    break;
                default:
                    mainMenuCanvasController.ShowMessage("There was an unknown problem while joining the lobby. Please try again.");
                    break;
            }
            Debug.LogException(ex);
        }
    }

    private async void RefreshLobbyList()
    {
        try
        {
            QueryResponse response = await lobbyController.ListLobbies();
            Button lobbyButton;
            ClearLobbyList();
            foreach (Lobby lobby in response.Results)
            {
                lobbyButton = Instantiate(lobbyTemplate, lobbyList);
                lobbyButton.transform.GetChild(0).GetComponent<TMP_Text>().text = lobby.Name;
                lobbyButton.transform.GetChild(1).GetComponent<TMP_Text>().text = lobby.MaxPlayers - lobby.AvailableSlots + " / " + lobby.MaxPlayers;
                lobbyButton.onClick.AddListener(() =>
                {
                    JoinLobby(lobby.Id);
                });
                lobbyButton.gameObject.SetActive(true);
            }
        }
        catch (LobbyServiceException ex)
        {
            switch (ex.Reason)
            {
                case LobbyExceptionReason.NetworkError:
                    mainMenuCanvasController.ShowMessage("Check your internet connection.");
                    break;
                default:
                    mainMenuCanvasController.ShowMessage("There was an unknown problem while refreshing the lobby list. Please try again.");
                    break;
            }
            Debug.LogException(ex);
        }
    }

    private IEnumerator ButtonCooldown(Button button, float cooldownTime)
    {
        button.interactable = false;
        yield return new WaitForSeconds(cooldownTime);
        button.interactable = true;
    }
}
