using UnityEngine;
using System.Collections;

public class SwordAttack : MonoBehaviour
{
    public int damage = 25;  // Amount of damage dealt by the sword
    public float attackRange = 9f;  // Range of the sword attack
    private Animator animator;
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Trigger attack animation on mouse1 press
        if (Input.GetButtonDown("Fire1") && !isAttacking)
        {
            Attack();
        }
    }

    void Attack()
    {
        // isAttacking = true;
        // animator.SetTrigger("Attack");

        // Detect all colliders in range of the attack
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        Debug.Log("colliders: " + hitColliders.Length);

        // Damage each collider that has an HPSystem component
        foreach (Collider collider in hitColliders)
        {
            EnemyHp hpSystem = collider.GetComponent<EnemyHp>();
            Debug.Log("Trying to damage: " + collider.gameObject.name);
            if (hpSystem != null)
            {
                hpSystem.TakeDamage(damage);
                Debug.Log($"Damaged enemy: {collider.gameObject.name}");
            }
        }

        // Reset isAttacking flag when the attack animation is finished
        // StartCoroutine(ResetAttack());
    }

    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}