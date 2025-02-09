using UnityEngine;

public class KnightAnimController : MonoBehaviour
{
    private Animator animator;
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        // Update movement parameters in the animator
        animator.SetFloat("Strafe", x);
        animator.SetFloat("Forward", y);

        // Trigger attack animation on mouse1 press
        if (Input.GetButtonDown("Fire1") && !isAttacking)
        {
            Debug.Log("Attacking");
            animator.CrossFade("Attack", 0f);
            isAttacking = true;
        }

        // Reset isAttacking flag when the attack animation is finished
        if (isAttacking && animator.GetCurrentAnimatorStateInfo(0).IsName("Movement"))
        {
            Debug.Log("Setting isAttacking to false");
            isAttacking = false;
        }
    }
}