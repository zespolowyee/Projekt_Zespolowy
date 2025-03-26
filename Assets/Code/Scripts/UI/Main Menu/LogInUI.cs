using System;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class LogInUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private MainMenuCanvasController mainMenuCanvasController;

    public void Awake()
    {
        loginButton.onClick.AddListener(OnLogInButtonClicked);
        usernameInput.onValueChanged.AddListener(OnUsernameInputChanged);
    }

    public void OnUsernameInputChanged(string newValue)
    {
        if(string.IsNullOrEmpty(newValue))
            loginButton.interactable = false;
        else
        {
            usernameInput.text = newValue.Trim();
            loginButton.interactable = true;
        }
    }

    private async void OnLogInButtonClicked()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            await AuthenticationService.Instance.UpdatePlayerNameAsync(usernameInput.text);
            gameObject.SetActive(false);
        }
        catch (AuthenticationException ex)
        {
            if (ex.ErrorCode == AuthenticationErrorCodes.InvalidParameters)
                mainMenuCanvasController.ShowMessage("Invalid username");
            else if (ex.ErrorCode == AuthenticationErrorCodes.BannedUser)
                mainMenuCanvasController.ShowMessage("You are banned");
            else
                mainMenuCanvasController.ShowMessage("There was an unknown error while logging in.");
            
            AuthenticationService.Instance.SignOut();
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            if (ex.ErrorCode == CommonErrorCodes.TransportError)
                mainMenuCanvasController.ShowMessage("Check your internet connection.");
            else
                mainMenuCanvasController.ShowMessage("There was an unknown error while logging in.");
            
            AuthenticationService.Instance.SignOut();
            Debug.LogException(ex);
        }
        catch (Exception ex)
        {
            mainMenuCanvasController.ShowMessage("There was an unknown error while logging in.");
            
            AuthenticationService.Instance.SignOut();
            Debug.LogException(ex);
        }
    }

}
