using TMPro;
using UnityEngine;

public class PlayerUpgradeNodeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI upgradeName;
    private UpgradeNode node;
    private Transform parentTransform;
    [SerializeField] private GameObject lineInPoint;
    [SerializeField] private GameObject lineOutPoint;

    public TextMeshProUGUI UpgradeName { get => upgradeName; set => upgradeName = value; }
    public UpgradeNode Node { get => node; set => node = value; }
    public Transform ParentTransform { get => parentTransform; set => parentTransform = value; }
    public GameObject LineOutPoint { get => lineOutPoint; set => lineOutPoint = value; }
    public GameObject LineInPoint { get => lineInPoint; set => lineInPoint = value; }
}
