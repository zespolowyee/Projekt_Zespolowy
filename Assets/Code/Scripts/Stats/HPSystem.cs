using UnityEngine;

public class HPSystem : MonoBehaviour
{
    public int maxHP = 100;
    private bool isDead = false;

    public Animator animator;
    private int currentHP;

    protected virtual void Start()
    {
        currentHP = maxHP;
        animator = GetComponent<Animator>();
    }

    public virtual void Update()
    {
        if (!isDead && Input.GetKeyDown(KeyCode.P) && animator != null)
        {
                TakeDamage(100);
        }
    }

    public virtual void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }
    }

    protected virtual void Die()
    {
        isDead = true;
        if (animator != null)
        {
            animator.Play("Die", -1, 0f);
            Debug.Log($"{gameObject.name} has died.");
        }
    }

    
}