using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CannonballScript : NetworkBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (IsServer)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                StartCoroutine(DestroyAfterDelay(0.2f));
            }
        }
    }
    
    private IEnumerator DestroyAfterDelay(float delay)
    {
        // Wait for the network state to synchronize across all clients
        yield return new WaitForSeconds(delay);

        // Deactivate the object after the delay
        Destroy(gameObject);
    }
    
    public void Start()
    {
        if (IsServer)
        {
            Destroy(gameObject, 10);
        }
    }
}
