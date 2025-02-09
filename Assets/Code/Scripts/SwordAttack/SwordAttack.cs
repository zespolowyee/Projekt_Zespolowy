using UnityEngine;
using System.Collections;

public class SwordAttack : MonoBehaviour
{
    private int damage;  // Amount of damage dealt by the sword
    private float attackRange;  // Range of the sword attack
    private Animator animator;
    private bool isAttacking = false;
    private bool isDead;

    void Start()
    {
        animator = GetComponent<Animator>();
        PlayerClass playerClass = GetComponent<PlayerClass>();
        if (playerClass != null)
        {
            damage = playerClass.damage;
            attackRange = playerClass.attackRange;
        }
        HPSystem hpSystem = GetComponent<HPSystem>();
        if (hpSystem != null)
        {
            isDead = hpSystem.isDead;
        }
    }

    void Update()
    {
        if(isDead) return;
        
        // Trigger attack animation on mouse1 press
        if (Input.GetButtonDown("Fire1") && !isAttacking)
        {
            Attack();
        }
    }

    void Attack()
    {
        isAttacking = true;
        animator.CrossFade("Attack", 0f);

        // Detect all colliders in range of the attack
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        // Damage each collider that has an EnemyHP component
        foreach (Collider collider in hitColliders)
        {
            EnemyHp enemyHp = collider.GetComponent<EnemyHp>();
            if (enemyHp != null)
            {
                enemyHp.TakeDamage(damage);
                Debug.Log($"Damaged enemy: {collider.gameObject.name}");
            }
        }

        // Start coroutine to reset isAttacking after the attack animation is finished
        StartCoroutine(ResetAttack());
    }

    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}