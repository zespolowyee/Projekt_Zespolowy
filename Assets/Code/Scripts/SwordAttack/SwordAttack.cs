using UnityEngine;
using System.Collections;
using Unity.Netcode;

// Ten skrypt jest odpowiedzialny za atakowanie przeciwników za pomocą miecza.
// Dodawaj go do prefabów będących mele postaciami gracza.
// W Animation Controller musisz użyć animacji "Attack" z triggerem "Attack".
public class SwordAttack : NetworkBehaviour
{
    private int damage;  // Amount of damage dealt by the sword
    private float attackRange;  // Range of the sword attack
    private ClientNetworkAnimator networkAnimator;
    private bool isAttacking = false;

    void Start()
    {
        if (!IsOwner)
        {
            enabled = false;
        }
        networkAnimator = GetComponent<ClientNetworkAnimator>();
        PlayerClass playerClass = GetComponent<PlayerClass>();
        if (playerClass != null)
        {
            damage = playerClass.damage;
            attackRange = playerClass.attackRange;
        }
    }

    void Update()
    {
        // If the player is dead, don't attack
        HPSystem hpSystem = GetComponent<HPSystem>();
        if (hpSystem != null && hpSystem.isDead)
        {
            return;
        }
        
        // Trigger attack on mouse1 press
        if (Input.GetButtonDown("Fire1") && !isAttacking)
        {
            Attack();
        }
    }

    void Attack()
    {
        isAttacking = true;
        networkAnimator.Animator.CrossFade("Attack", 0f);

        // Detect all colliders in range of the attack
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        // Damage each collider that has an EnemyHP component
        foreach (Collider collider in hitColliders)
        {
            EnemyHp enemyHp = collider.GetComponent<EnemyHp>();
            if (enemyHp != null)
            {
                enemyHp.TakeDamageFromSource(damage, gameObject);
                Debug.Log($"Damaged enemy: {collider.gameObject.name}");
            }
        }

        // Start coroutine to reset isAttacking after the attack animation is finished
        StartCoroutine(ResetAttack());
    }

    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(networkAnimator.Animator.GetCurrentAnimatorStateInfo(0).length);
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}