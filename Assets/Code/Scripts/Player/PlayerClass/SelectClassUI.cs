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
        knightButton.onClick.AddListener(() =>
        {
            OnClassSelected?.Invoke("Knight");
        });

        hunterButton.onClick.AddListener(() =>
        {
            OnClassSelected?.Invoke("Hunter");
        });
    }
}
