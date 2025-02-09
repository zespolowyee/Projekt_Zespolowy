using UnityEngine;

public class KnightController : MonoBehaviour
{
    private Animator animator;
    private bool isDead;

    void Start()
    {
        animator = GetComponent<Animator>();
        HPSystem hpSystem = GetComponent<HPSystem>();
        if (hpSystem != null)
        {
            isDead = hpSystem.isDead;
        }
    }

    void Update()
    {
        if(isDead) return;
        
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        // Update movement parameters in the animator
        animator.SetFloat("Strafe", x);
        animator.SetFloat("Forward", y);
    }
}