using TMPro;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class CreateLobbyUI : MonoBehaviour
{
    [SerializeField] private int maxMaxPlayers;
    [SerializeField] private Button backButton;
    [SerializeField] private Button createButton;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField maxPlayersInput;
    [SerializeField] private Toggle privateLobbyToggle;
    [SerializeField] private MainMenuCanvasController mainMenuCanvasController;
    [SerializeField] private LobbyController lobbyController;
    
    public void Awake()
    {
        backButton.onClick.AddListener(OnBackButtonClicked);
        createButton.onClick.AddListener(OnCreateButtonClicked);
        maxPlayersInput.onValueChanged.AddListener(OnMaxPlayersInputChanged);
        
        nameInput.onValueChanged.AddListener(OnAnyValueChanged);
        maxPlayersInput.onValueChanged.AddListener(OnAnyValueChanged);
    }

    private void OnAnyValueChanged(string newValue)
    {
        createButton.interactable = !string.IsNullOrEmpty(nameInput.text) && !string.IsNullOrEmpty(maxPlayersInput.text);
    }

    private void OnMaxPlayersInputChanged(string newValue)
    {
        if (string.IsNullOrEmpty(newValue))
        {
            return;
        }
        
        int maxPlayers = int.Parse(newValue);
        if (maxPlayers > maxMaxPlayers)
        {
            maxPlayersInput.text = maxMaxPlayers.ToString();
        }
    }

    private async void OnCreateButtonClicked()
    {
        try
        { 
            await lobbyController.CreateLobby(nameInput.text, int.Parse(maxPlayersInput.text), privateLobbyToggle.isOn);
            mainMenuCanvasController.ShowLobby();
        }
        catch
        {
            mainMenuCanvasController.ShowMessage("There was a problem creating the lobby. Please try again.");
            throw;
        }
    }
    
    private void OnBackButtonClicked()
    {
        gameObject.SetActive(false);
    }
}