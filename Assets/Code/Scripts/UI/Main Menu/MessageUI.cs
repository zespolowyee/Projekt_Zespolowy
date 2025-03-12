using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button closeButton;

    public void Awake()
    {
        closeButton.onClick.AddListener(OnCloseButtonClicked);
    }

    public void ChangeText(string text)
    {
        messageText.text = text;
    }

    public void OnCloseButtonClicked()
    {
        gameObject.SetActive(false);
    }
}
