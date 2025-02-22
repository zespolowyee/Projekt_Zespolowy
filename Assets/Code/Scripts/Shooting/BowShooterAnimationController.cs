using UnityEngine;
using Unity.Netcode;

public class BowShooterAnimationController : NetworkBehaviour
{
    private Animator animator;
    private bool isDrawing = false;
    public bool IsDrawing => isDrawing;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!IsOwner) return;

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        animator.SetFloat("Strafe", x);
        animator.SetFloat("Forward", y);

        // Naciąganie łuku
        if (Input.GetButtonDown("Fire1"))
        {
            isDrawing = true;
            SetDrawingStateServerRpc(true);        }

        // Strzelanie po zwolnieniu przycisku
        if (Input.GetButtonUp("Fire1"))
        {
            SetShootingTriggerServerRpc();
        }

        // Resetowanie parametrów po zakończeniu animacji "Shoot Arrow"
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Shoot"))
        {
            isDrawing = false;
            SetDrawingStateServerRpc(false);
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                ResetShootTriggerClientRpc();
            }
        }
    }
    [ServerRpc]
    private void SetDrawingStateServerRpc(bool state)
    {
        isDrawing = state;
        SetDrawingStateClientRpc(state);
    }

    [ClientRpc]
    private void SetDrawingStateClientRpc(bool state)
    {
        animator.SetBool("IsDrawing", state);
    }

    [ServerRpc]
    private void SetShootingTriggerServerRpc()
    {
        SetShootingTriggerClientRpc();
    }

    [ClientRpc]
    private void SetShootingTriggerClientRpc()
    {
        animator.SetTrigger("Shoot");
    }
    [ClientRpc]
    private void ResetShootTriggerClientRpc()
    {
        animator.ResetTrigger("Shoot");
    }
}
