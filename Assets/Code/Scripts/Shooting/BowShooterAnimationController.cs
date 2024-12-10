using UnityEngine;

public class BowShooterAnimationController : MonoBehaviour
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
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        // Aktualizacja parametrów ruchu w animatorze
        animator.SetFloat("Strafe", x);
        animator.SetFloat("Forward", y);

        // Naciąganie łuku
        if (Input.GetButtonDown("Fire1"))
        {
            isDrawing = true;
            animator.SetBool("IsDrawing", true);
        }

        // Strzelanie po zwolnieniu przycisku
        if (Input.GetButtonUp("Fire1"))
        {
            animator.SetTrigger("Shoot");  // Używamy triggera zamiast boola
        }

        // Resetowanie parametrów po zakończeniu animacji "Shoot Arrow"
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Shoot"))
        {
            isDrawing = false;
            animator.SetBool("IsDrawing", false);
            // Możesz tutaj sprawdzić, czy animacja trwa dłużej niż pewien czas i zresetować trigger tylko wtedy, gdy animacja się skończy
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                animator.ResetTrigger("Shoot");
            }
        }
    }
}
