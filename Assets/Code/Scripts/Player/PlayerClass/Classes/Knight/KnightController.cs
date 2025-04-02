using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class KnightController : NetworkBehaviour
{
    private ClientNetworkAnimator animator;

    void Start()
    {
        if (!IsOwner)
        {
            enabled = false;
        }
        animator = GetComponent<ClientNetworkAnimator>();
    }

    void Update()
    {
        // If the player is dead, stop the player animations
        HPSystem hpSystem = GetComponent<HPSystem>();
        if (hpSystem != null && hpSystem.isDead)
        {
            return;
        }
        
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        // Update movement parameters in the animator
        animator.Animator.SetFloat("Strafe", x);
        animator.Animator.SetFloat("Forward", y);
    }
}