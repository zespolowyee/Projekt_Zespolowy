using UnityEngine;
using UnityEngine.UI;

public class SelectClassUI : MonoBehaviour
{
    [SerializeField] private Button knightButton;
    [SerializeField] private Button paladinButton;

    public delegate void ClassSelected(string className);
    public static event ClassSelected OnClassSelected;

    private void Awake()
    {
        // Add logging for Knight button click
        knightButton.onClick.AddListener(() =>
        {
            OnClassSelected?.Invoke("Knight");
        });

        // Add logging for Hunter button click
        paladinButton.onClick.AddListener(() =>
        {
            OnClassSelected?.Invoke("Paladin");
        });
    }
}