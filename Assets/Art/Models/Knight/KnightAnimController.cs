using UnityEngine;

public class KnightAnimController : MonoBehaviour
{
    private Animator animator;

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
    }
}