using UnityEngine;
using UnityEngine.UI;

public class SelectClassUI : MonoBehaviour
{
    [SerializeField] private Button knightButton;
    [SerializeField] private Button hunterButton;

    public delegate void ClassSelected(string className);
    public static event ClassSelected OnClassSelected;

    private void Awake()
    {
        // Log a message to confirm the script is running
        Debug.Log("SelectClassUI script initialized.");

        // Add logging for Knight button click
        knightButton.onClick.AddListener(() =>
        {
            Debug.Log("Knight button clicked.");
            OnClassSelected?.Invoke("Knight");
        });

        // Add logging for Hunter button click
        hunterButton.onClick.AddListener(() =>
        {
            Debug.Log("Hunter button clicked.");
            OnClassSelected?.Invoke("Hunter");
        });
    }
}