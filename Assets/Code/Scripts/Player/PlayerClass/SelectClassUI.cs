using UnityEngine;
using UnityEngine.UI;

public class SelectClassUI : MonoBehaviour
{
    [SerializeField] private Button knightButton;
    [SerializeField] private Button axemanButton;

    public delegate void ClassSelected(PlayerClassType playerClass);
    public static event ClassSelected OnClassSelected;

    private void Awake()
    {
        // Add logging for Knight button click
        knightButton.onClick.AddListener(() =>
        {
            OnClassSelected?.Invoke(PlayerClassType.Knight);
        });

        // Add logging for Hunter button click
        axemanButton.onClick.AddListener(() =>
        {
            OnClassSelected?.Invoke(PlayerClassType.Axeman);
        });
    }
}