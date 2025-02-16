using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class XPSystem : NetworkBehaviour
{
    private NetworkVariable<int> currentEXP = new NetworkVariable<int>(
        1000, 
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server 
    );

    public TextMeshProUGUI xpText;

    private void Start()
    {
        if (IsClient)
        {
            currentEXP.OnValueChanged += UpdateUI;
        }

        if (IsOwner)
        {
            UpdateUI(0, currentEXP.Value);
        }
    }

    public int GetCurrentEXP()
    {
        return currentEXP.Value;
    }

    public void DeductEXP(int amount)
    {
        if (!IsServer) return;
        if (amount > currentEXP.Value)
        {
            Debug.LogError("Not enough EXP!");
            return;
        }

        currentEXP.Value -= amount;
        Debug.Log($"EXP deducted: {amount}. Remaining EXP: {currentEXP.Value}");
    }

    public void AddEXP(int amount)
    {
        if (!IsServer) return;
        currentEXP.Value += amount;
        Debug.Log($"Gained {amount} EXP! Total EXP: {currentEXP.Value}");
    }

    private void UpdateUI(int oldValue, int newValue)
    {
        if (xpText != null)
        {
            xpText.text = $"EXP: {newValue}";
        }
    }
}