using Unity.Netcode;
using UnityEngine;

public class ChickenExplodeAttack : EnemyAttack
{
    [SerializeField] Explosion explosion;
    [SerializeField] EnemyHp hp;
    bool exploded = false;
    public override void SetupAttack(EnemyNavigation controller)
    {
        base.SetupAttack(controller);
        explosion = GetComponent<Explosion>();

    }
    public override void PerformAttack()
    {
        if (!IsServer)
        {
            ExplodeServerRpc();
        }
        else
        {
            if (!exploded)
            {
                StartCoroutine(explosion.Explode());
                exploded = true;
            }
            DetachParticlesRpc();
            transform.parent = null;
            Destroy(gameObject, 1f);
            hp.Die();
        }


    }

    [Rpc(SendTo.Server)]
    public void ExplodeServerRpc()
    {
        if (!exploded)
        {
            StartCoroutine(explosion.Explode());
            exploded = true;
        }
        DetachParticlesRpc();
        transform.parent = null;
        Destroy(gameObject, 1f);
        hp.Die();

    }

    [Rpc(SendTo.NotServer)]
    public void DetachParticlesRpc()
    {
        transform.parent = null;
        Destroy(gameObject, 1f);
    }
}
