using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayMenuUI : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button createButton;
    [SerializeField] private Button findButton;
    [SerializeField] private Button joinWithCodeButton;
    [SerializeField] private TMP_InputField joinWithCodeInput;
    [SerializeField] private MainMenuCanvasController mainMenuCanvasController;
    
    public void Awake()
    {
        joinWithCodeButton.onClick.AddListener(OnJoinWithCodeClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void OnJoinWithCodeClicked()
    {
        mainMenuCanvasController.ShowMessage(joinWithCodeInput.text);
    }
    
    private void OnBackButtonClicked()
    {
        gameObject.SetActive(false);
    }
}
