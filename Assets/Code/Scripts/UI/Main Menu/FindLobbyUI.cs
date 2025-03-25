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
        catch
        {
            mainMenuCanvasController.ShowMessage("There was a problem joining the lobby. Please try again.");
            throw;
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
        catch
        {
            mainMenuCanvasController.ShowMessage("There was a problem refreshing the lobby list. Please try again.");
        }
    }

    private IEnumerator ButtonCooldown(Button button, float cooldownTime)
    {
        button.interactable = false;
        yield return new WaitForSeconds(cooldownTime);
        button.interactable = true;
    }
}
