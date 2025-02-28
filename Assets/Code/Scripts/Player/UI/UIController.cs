using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject turretUpgradeUI;
    
    private List<GameObject> openedWindows;
    private bool isInputLocked = false;

    public void Awake()
    {
        openedWindows = new List<GameObject>();
    }

    public void LockUserInput()
    {
        gameObject.GetComponentInChildren<RBController>().enabled = false;
        gameObject.GetComponentInChildren<PlayerInteract>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
        isInputLocked = true;
    }
    
    public void UnlockUserInput()
    {
        gameObject.GetComponentInChildren<RBController>().enabled = true;
        gameObject.GetComponentInChildren<PlayerInteract>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        isInputLocked = false;
    }

    public GameObject OpenWindow(GameObject window)
    {
        GameObject windowObject = Instantiate(window);
        
        Window windowScript = windowObject.GetComponent<Window>();
        
        windowScript.player = gameObject;
        windowScript.OnClose.AddListener(() => CloseWindow(windowObject));

        openedWindows.Add(windowObject);
        
        if (!isInputLocked)
            LockUserInput();

        return windowObject;
    }
    
    public void CloseWindow(GameObject window)
    {
        openedWindows.Remove(window);
        
        Destroy(window);
        
        if (openedWindows.Count == 0 && isInputLocked)
            UnlockUserInput();
    }

    public void DisplayTurretUpgradeUI(TurretStats turretStats)
    {
        GameObject upgradeUIWindow = OpenWindow(turretUpgradeUI);
        upgradeUIWindow.GetComponent<TurretUpgradeUIWindow>().turretStats = turretStats;
    }
}
