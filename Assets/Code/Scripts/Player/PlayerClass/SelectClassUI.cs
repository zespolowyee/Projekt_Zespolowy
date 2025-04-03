using UnityEngine;
using UnityEngine.UI;

public class SelectClassUI : MonoBehaviour
{
    [SerializeField] private Button knightButton;
    [SerializeField] private Button paladinButton;
    [SerializeField] private Button axemanButton;

    public delegate void ClassSelected(PlayerClassType playerClass);
    public static event ClassSelected OnClassSelected;

    private void Awake()
    {
        knightButton.onClick.AddListener(() =>
        {
            OnClassSelected?.Invoke(PlayerClassType.Knight);
        });

        paladinButton.onClick.AddListener(() =>
        {
            OnClassSelected?.Invoke(PlayerClassType.Paladin);
        });

        axemanButton.onClick.AddListener(() =>
        {
            OnClassSelected?.Invoke(PlayerClassType.Axeman);
        });

    }
}