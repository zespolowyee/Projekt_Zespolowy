using UnityEngine;
using Unity.Netcode;
public class ArrowHit : NetworkBehaviour
{
    public int damage = 25;  // Ilość obrażeń zadawanych przez strzałę
    private Rigidbody rb;
    private bool hasHit = false;  // Flaga, aby upewnić się, że strzała zatrzymuje się tylko raz
    private GameObject attacker;

    public void SetAttacker(GameObject attackerObject)
    {
        attacker = attackerObject;
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ServerTakeDamageServerRpc(ulong enemyNetworkObjectId)
    {
        // Szukamy obiektu po jego NetworkObjectId
        NetworkObject enemyNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[enemyNetworkObjectId];
        
        if (enemyNetworkObject != null)
        {
            EnemyHp enemyHP = enemyNetworkObject.GetComponent<EnemyHp>();
            if (enemyHP != null)
            {
                enemyHP.TakeDamageFromSource(damage, attacker);
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void ServerSetHitStateServerRpc(bool state)
    {
        hasHit = state;
    }

    private void OnCollisionEnter(Collision collision)
    {
         if (hasHit) return;  // Jeśli już trafiono, zakończ funkcję

        hasHit = true;  // Ustaw flagę na true po pierwszym trafieniu
        ServerSetHitStateServerRpc(true);

        EnemyHp enemyHP = collision.gameObject.GetComponent<EnemyHp>();
        if (enemyHP != null)
        {
            if (IsServer)
            {
                enemyHP.TakeDamageFromSource(damage, attacker);
            }
            else
            {
                NetworkObject enemyNetworkObject = collision.gameObject.GetComponent<NetworkObject>();
                if (enemyNetworkObject != null)
                {
                    ServerTakeDamageServerRpc(enemyNetworkObject.NetworkObjectId);
                }
            }
        }
        else
        {
            Debug.Log($"Trafiono w {gameObject.name}");
        }

        rb.isKinematic = true;  

        NetworkObject networkObject = collision.gameObject.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            transform.parent = collision.transform;
        }

        Destroy(gameObject, 1f);
    }
}



