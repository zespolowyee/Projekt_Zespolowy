using Unity.VisualScripting;
using UnityEngine;

public class MainMenuCanvasController : MonoBehaviour
{ 
    [SerializeField] private Canvas playMenuCanvas;
    [SerializeField] private Canvas messageCanvas;

    public void ShowPlayMenu()
    {
        playMenuCanvas.gameObject.SetActive(true);
    }

    public void ShowMessage(string message)
    {
        messageCanvas.GetComponent<MessageUI>().ChangeText(message);
        messageCanvas.gameObject.SetActive(true);
    }
}
