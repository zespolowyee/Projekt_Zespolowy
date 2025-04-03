using TMPro;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UI;

public class PlayMenuUI : MonoBehaviour
{
    [SerializeField] private int codeLenght;
    [SerializeField] private Button backButton;
    [SerializeField] private Button createButton;
    [SerializeField] private Button findButton;
    [SerializeField] private Button joinWithCodeButton;
    [SerializeField] private TMP_InputField joinWithCodeInput;
    [SerializeField] private MainMenuCanvasController mainMenuCanvasController;
    [SerializeField] private LobbyController lobbyController;
    
    public void Awake()
    {
        joinWithCodeButton.onClick.AddListener(OnJoinWithCodeClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
        joinWithCodeInput.onValueChanged.AddListener(OnJoinWithCodeInputChanged);
        createButton.onClick.AddListener(OnCreateButtonClicked);
        findButton.onClick.AddListener(OnFindButtonClicked);
    }

    private void OnCreateButtonClicked()
    {
        mainMenuCanvasController.ShowCreateLobby();
    }
    
    private void OnFindButtonClicked()
    {
        mainMenuCanvasController.ShowFindLobby();
    }

    private void OnJoinWithCodeInputChanged(string input)
    {
        joinWithCodeButton.interactable = !(input.Length < codeLenght);
    }

    private async void OnJoinWithCodeClicked()
    {
        try
        {
            await lobbyController.JoinLobbyWithCode(joinWithCodeInput.text);
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
                case LobbyExceptionReason.InvalidJoinCode:
                    mainMenuCanvasController.ShowMessage("This join code is invalid.");
                    break;
                case LobbyExceptionReason.LobbyNotFound:
                    mainMenuCanvasController.ShowMessage("This lobby cannot be found.");
                    break;
                default:
                    mainMenuCanvasController.ShowMessage("There was an unknown problem while joining the lobby. Please try again.");
                    break;
            }
        }
    }
    
    private void OnBackButtonClicked()
    {
        gameObject.SetActive(false);
    }
}
