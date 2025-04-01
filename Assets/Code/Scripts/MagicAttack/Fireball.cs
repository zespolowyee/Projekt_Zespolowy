using Unity.Netcode;
using UnityEngine;

// Ten skrypt obsługuje zachowanie kuli ognia, w tym eksplozję i zadawanie obrażeń obszarowych.
public class Fireball : NetworkBehaviour
{
    public GameObject explosionEffect; // Prefab efektu eksplozji
    private int damage;
    private float explosionRadius;

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    public void SetExplosionRadius(float radius)
    {
        explosionRadius = radius;
    }

    void OnCollisionEnter(Collision collision)
    {
        ExplodeServerRpc();
    }

    [ServerRpc]
    void ExplodeServerRpc()
    {
        Explode();
    }

    void Explode()
    {
        // Tworzenie efektu eksplozji
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
        
        // Wyszukiwanie obiektów w zasięgu eksplozji
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hitColliders)
        {
            EnemyHp enemyHp = hit.GetComponent<EnemyHp>();
            if (enemyHp != null)
            {
                enemyHp.TakeDamageFromSource(damage, gameObject);
                Debug.Log($"Trafiono: {hit.gameObject.name}, zadano {damage} obrażeń");
            }
        }
        
        // Usunięcie kuli ognia po eksplozji
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}