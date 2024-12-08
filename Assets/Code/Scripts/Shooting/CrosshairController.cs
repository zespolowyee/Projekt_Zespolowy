using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
   [SerializeField] private GameObject crosshair;  // Referencja do obrazu celownika
   private GameObject crosshairInstance; 
   [SerializeField] private Canvas uiCanvas;

    void Start()
    {
        if (crosshair != null)
        {
            crosshairInstance = Instantiate(crosshair, uiCanvas.transform);
            crosshairInstance.SetActive(true);  // Celownik widoczny na początku
        }
    }
}