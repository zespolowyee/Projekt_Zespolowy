using Unity.VisualScripting;
using UnityEngine;

public class MainMenuCanvasController : MonoBehaviour
{ 
    [SerializeField] private Canvas playMenuCanvas;
    [SerializeField] private Canvas createLobbyCanvas;
    [SerializeField] private Canvas findLobbyCanvas;
    [SerializeField] private Canvas lobbyCanvas;
    [SerializeField] private Canvas messageCanvas;

    public void ShowPlayMenu()
    {
        playMenuCanvas.gameObject.SetActive(true);
    }

    public void ShowCreateLobby()
    {
        createLobbyCanvas.gameObject.SetActive(true);
    }
    
    public void ShowFindLobby()
    {
        findLobbyCanvas.gameObject.SetActive(true);
    }
    
    public void ShowLobby()
    {
        lobbyCanvas.gameObject.SetActive(true);
    }

    public void ShowMessage(string message)
    {
        messageCanvas.GetComponent<MessageUI>().ChangeText(message);
        messageCanvas.gameObject.SetActive(true);
    }
}
