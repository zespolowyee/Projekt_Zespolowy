using Unity.Netcode;
using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Rigidbody))]
public class Fireball : NetworkBehaviour
{ private int damage;
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
}
