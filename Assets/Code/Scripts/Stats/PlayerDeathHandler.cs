using Unity.Netcode;
using UnityEngine;

public class PlayerDeathHandler : NetworkBehaviour
{
    private GameObject deathScreenUI;
    private RBController rbController;

    private void Start()
    {
        deathScreenUI = GameObject.Find("DeathScreen");
        rbController = GetComponent<RBController>();
        
        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(false);
        }
    }

    public void HandleDeath()
    {
        if (IsOwner)
        {
            DisablePlayerControls();
            ShowDeathScreen();
        }
    }

    private void DisablePlayerControls()
    {
        if (rbController != null)
        {
            rbController.enabled = false;
        }

        var attack = GetComponent<SwordAttack>();
        if (attack != null)
        {
            attack.enabled = false;
        }
        
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        var capsuleCollider = GetComponent<CapsuleCollider>();
        if (capsuleCollider != null)
        {
            capsuleCollider.enabled = false;
        }
    }

    private void ShowDeathScreen()
    {
        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(true);
        }
    }
}