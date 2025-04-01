using UnityEngine;
using System.Collections;
using Unity.Netcode;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;

// Ten skrypt odpowiada za atakowanie przeciwników za pomocą kuli ognia.
// Dodawaj go do prefabów będących postaciami gracza używającymi magii.
// W Animation Controller musisz użyć animacji "CastSpell" z triggerem "Cast".
public class MagicAttack : NetworkBehaviour
{
    public GameObject fireballPrefab; // Prefab kuli ognia
    public Transform firePoint; // Ustaw to w Inspectorze w Unity

    public float fireballSpeed = 5f; // Prędkość kuli ognia
    private int damage; // Obrażenia zadawane przez kulę ognia
    private float attackRange;
    
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
        // Sprawdzenie, czy gracz nie jest martwy
        HPSystem hpSystem = GetComponent<HPSystem>();
        if (hpSystem != null && hpSystem.isDead)
        {
            return;
        }
        
        // Wyzwolenie ataku na kliknięcie przycisku myszy (Fire1)
        if (Input.GetButtonDown("Fire1") && !isAttacking)
        {
            Attack();
        }
    }
    void Attack()
    {
        isAttacking = true;
        networkAnimator.Animator.CrossFade("Attack", 0f);

        // Rozpocznij korutynę, która poczeka na zakończenie animacji i wywoła CastFireballServerRpc
        CastFireballServerRpc();
        
        // Start coroutine to reset isAttacking after the attack animation is finished
        StartCoroutine(ResetAttack());
    }

    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(0.4f);
        yield return new WaitForSeconds(networkAnimator.Animator.GetCurrentAnimatorStateInfo(0).length);
        isAttacking = false;
    }

    IEnumerator WaitForAnimationAndCastFireball(float duration)
    {
        yield return new WaitForSeconds(duration);
        Vector3 spawnPosition = firePoint.position; 
        GameObject fireball = Instantiate(fireballPrefab, spawnPosition, firePoint.rotation);

        NetworkObject networkObject = fireball.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn(); // Synchronizuj obiekt w sieci
        }

        Rigidbody rb = fireball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * fireballSpeed; // Kula ognia leci do przodu
        }

        Fireball fireballScript = fireball.GetComponent<Fireball>();
        if (fireballScript != null)
        {
            fireballScript.SetDamage(damage);
            fireballScript.SetExplosionRadius(attackRange);
        }
    }

    [ServerRpc]
    void CastFireballServerRpc()
    {
        StartCoroutine(WaitForAnimationAndCastFireball(1.0f));
    }

}