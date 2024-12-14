using UnityEngine;

public class HPSystem : MonoBehaviour
{
    public int maxHP = 100;
    private bool isDead = false;

    public Animator animator;
    private int currentHP;

    private void Start()
    {
        currentHP = maxHP;
        animator = GetComponent<Animator>();
    }

    public void Update()
    {
        if (!isDead && Input.GetKeyDown(KeyCode.P) && animator != null)
        {
                TakeDamage(100);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        if (animator != null)
        {
            animator.Play("Die", -1, 0f);
            Debug.Log($"{gameObject.name} has died.");
        }
    }

    public bool IsDead()
    {
        return isDead;
    }
}