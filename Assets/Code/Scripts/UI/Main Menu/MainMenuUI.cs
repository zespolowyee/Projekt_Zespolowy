using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private MainMenuCanvasController mainMenuCanvasController;

    public void Awake()
    {
        playButton.onClick.AddListener(OnPlayButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    public void OnPlayButtonClicked()
    {
        mainMenuCanvasController.ShowPlayMenu();
    }

    public void OnExitButtonClicked()
    {
        Application.Quit();
    }
}
