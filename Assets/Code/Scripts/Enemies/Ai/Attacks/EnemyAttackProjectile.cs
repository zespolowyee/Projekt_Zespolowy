using Unity.Netcode;
using UnityEngine;

public class EnemyAttackProjectile : EnemyAttack
{
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected float projectileLifetime;

    public override void PerformAttack()
    {
        base.PerformAttack();
        if (IsServer)
        {
            ShootAtTargetClientRpc();
        }

    }

    void ShootAtTarget()
    {
        GameObject projectile = Instantiate(projectilePrefab, attackPos.position, Quaternion.identity);
        projectile.transform.forward = attackPos.transform.forward;
        Projectile projectileScript = projectile.GetComponent<Projectile>();

        projectileScript.WhatIsTarget = whatIsTarget;
        projectileScript.Damage = damage;
        projectileScript.Speed = projectileSpeed;
        projectileScript.Lifetime = projectileLifetime;


    }

    [Rpc(SendTo.ClientsAndHost)]
    void ShootAtTargetClientRpc()
    {
        ShootAtTarget();
    }
}
