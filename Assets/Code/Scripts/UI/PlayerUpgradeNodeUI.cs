using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUpgradeNodeUI : MonoBehaviour
{

    private UpgradeNode node;
    private Transform parentTransform;

    [SerializeField] private TextMeshProUGUI upgradeName;
    [SerializeField] private GameObject lineInPoint;
    [SerializeField] private GameObject lineOutPoint;
    [SerializeField] private Button button;
    [SerializeField] private GameObject unlockedIndicator;

    public TextMeshProUGUI UpgradeName { get => upgradeName; set => upgradeName = value; }
    public UpgradeNode Node { get => node; set => node = value; }
    public Transform ParentTransform { get => parentTransform; set => parentTransform = value; }
    public GameObject LineOutPoint { get => lineOutPoint; set => lineOutPoint = value; }
    public GameObject LineInPoint { get => lineInPoint; set => lineInPoint = value; }
    public Button Buton { get => button; set => button = value; }

    private void OnEnable()
    {
        PlayerUpgradeManager.OnUpgradeBought += UpdateIndicator;
    }

    private void OnDisable()
    {
        PlayerUpgradeManager.OnUpgradeBought -= UpdateIndicator;
    }

    public void DisplayData(UpgradeNode node, Transform parentTransform)
    {
        Node = node;
        UpgradeName.text = node.description;
        ParentTransform = parentTransform;
        unlockedIndicator.SetActive(node.isUnlocked);
    }

    public void UpdateIndicator()
    {
        unlockedIndicator.SetActive(node.isUnlocked);
    }

}
