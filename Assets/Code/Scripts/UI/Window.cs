using UnityEngine;
using UnityEngine.Events;

public class Window : MonoBehaviour, IWindow
{
    public UnityEvent OnClose;
    public GameObject player { set; get; }
    
    public void CloseWindow()
    {
        OnClose.Invoke();
    }
}